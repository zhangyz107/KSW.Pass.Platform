using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Instruments;
using KSW.ATE01.Pattern.Application.Models.Instruments;
using KSW.Exceptions;
using KSW.Helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace KSW.ATE01.Pattern.Application.BLLs.Implements.Instruments
{
    /// <summary>
    /// Udp设备控制服务
    /// </summary>
    public class UdpInstrumentControlService : InstrumentControlBaseService
    {
        #region Field
        private readonly IEventAggregator _eventAggregator;
        private ConcurrentDictionary<string, UdpClient> _connectionPool = new ConcurrentDictionary<string, UdpClient>();

        //发送队列
        private readonly ConcurrentQueue<SendMessageModel> _sendQueue = new ConcurrentQueue<SendMessageModel>();
        private readonly ConcurrentQueue<RecordMessageModel> _receiveQueue = new ConcurrentQueue<RecordMessageModel>();

        //发送队列最多支持命令数量
        private readonly int _maxSendCount = 100;

        #endregion
        public UdpInstrumentControlService(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            StartSendQueueTask();
        }

        public override IInstruentControlService CreateConnect(InstrumentInfoModel instrument)
        {
            try
            {
                if (instrument.Address.IsEmpty())
                    throw new Warning("设备地址不能为空");

                if (_connectionPool.Keys.Contains(instrument.Address))
                    return this;

                if (PraseIPAddress(instrument.Address, out string ipAddress, out int port, out int localPort))
                {
                    var tempInstrumentInfo = DeepCopy.Copy(instrument);
                    tempInstrumentInfo.IpAddress = ipAddress;
                    tempInstrumentInfo.Port = port;
                    tempInstrumentInfo.LocalPort = localPort;
                    ConnectInstrument(tempInstrumentInfo);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return this;
        }

        public override bool IsConnected(InstrumentInfoModel instrument)
        {
            return _connectionPool.Keys.Contains(instrument.Address);
        }

        public override IInstruentControlService DestroyConnect(InstrumentInfoModel instrument)
        {
            try
            {
                if (_connectionPool.TryRemove(instrument?.Address, out UdpClient udpClient) && udpClient != null)
                {
                    udpClient.Close();
                    udpClient = null;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return this;
        }

        public override void Send(InstrumentInfoModel instrument, byte[] data)
        {
            if (data == null || !data.Any()) { return; }

            if (_sendQueue.Count > _maxSendCount)
            {
                _sendQueue.TryDequeue(out SendMessageModel message);
                //todo:记录被剔除的消息
                //Log.LogInformation($"向设备{message.Address}发送内容:{message.Message.ToAppendString()}被剔除发送队列");
            }
            var sendMessage = new SendMessageModel()
            {
                Address = instrument.Address,
                Message = data,
                SendTimeOut = instrument.SendTimeOut,
                StringEncoder = instrument.StringEncoder,
            };
            _sendQueue.Enqueue(sendMessage);
        }

        public override void Send(InstrumentInfoModel instrument, string data)
        {
            if (string.IsNullOrEmpty(data)) { return; }

            if (_sendQueue.Count > _maxSendCount)
            {
                _sendQueue.TryDequeue(out SendMessageModel message);
                //todo:记录被剔除的消息
                Log.LogInformation($"向设备{message.Address}发送内容:{message.StringEncoder.GetString(message.Message)}被剔除发送队列");
            }
            var dataBytes = instrument.StringEncoder.GetBytes(data);
            var sendMessage = new SendMessageModel()
            {
                Address = instrument.Address,
                Message = dataBytes,
                SendTimeOut = instrument.SendTimeOut,
                StringEncoder = instrument.StringEncoder,
            };
            _sendQueue.Enqueue(sendMessage);
        }

        public override void SendLine(InstrumentInfoModel instrument, string data)
        {
            if (string.IsNullOrEmpty(data)) { return; }

            if (!data.EndsWith(instrument.StringEncoder.GetString(instrument.Delimiter)))
            {
                Send(instrument, data + instrument.StringEncoder.GetString(instrument.Delimiter));
            }
            else
            {
                Send(instrument, data);
            }
        }

        protected override bool ConnectInstrument(InstrumentInfoModel instrument)
        {
            var isConnected = false;
            try
            {
                if (instrument.IpAddress.IsEmpty())
                    throw new Warning("当前设备地址为空！");

                if (!instrument.IsMulticasst && !IsHostOnline(IPAddress.Parse(instrument.IpAddress)))
                    throw new ArgumentException($"当前IP地址:{instrument.IpAddress}无法Ping通！");

                var udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, instrument.LocalPort));
                var targetEndPoint = new IPEndPoint(IPAddress.Parse(instrument.IpAddress), instrument.Port);
                udpClient.Connect(targetEndPoint);
                if (instrument.IsMulticasst)
                {
                    udpClient.JoinMulticastGroup(targetEndPoint.Address);
                }
                udpClient.Client.SendBufferSize = int.MaxValue;
                udpClient.Client.ReceiveBufferSize = int.MaxValue;
                isConnected = true;

                _connectionPool.TryAdd(instrument.Address, udpClient);
                StartReceiveTask(instrument);
            }
            catch (Exception)
            {
                throw;
            }

            return isConnected;
        }

        /// <summary>
        /// 开启发送队列任务
        /// </summary>
        private void StartSendQueueTask()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (_sendQueue.TryDequeue(out SendMessageModel message) && message.Message.Any())
                            SendToInstrument(message.Address, message.Message, message.SendTimeOut);
                        else
                            await Task.Delay(10);

                    }
                    catch (Exception e)
                    {
                        //todo:记录日志 
                        Log.LogError(e, e.Message);
                    }
                }

            }, TaskCreationOptions.LongRunning);
        }

        private void SendToInstrument(string address, byte[] data, int timeOut)
        {
            try
            {
                var udpClient = _connectionPool[address];
                if (udpClient == null)
                {
                    throw new Warning("无法发送到空设备 (检查设备是否正常连接)！");
                }
                udpClient.Client.SendTimeout = timeOut;
                udpClient.Client.Send(data);

                //_eventAggregator?.GetEvent<SendMessageEvent>().Publish(new SendMessageModel { Address = address, Message = data });
            }
            catch
            {
                throw;
            }
        }

        private void StartReceiveTask(InstrumentInfoModel instrument)
        {
            Task.Factory.StartNew(async () =>
            {
                while (_connectionPool.ContainsKey(instrument.Address))
                {
                    try
                    {
                        var udpClient = _connectionPool[instrument.Address];
                        var result = await udpClient.ReceiveAsync();
                        if (result.Buffer.Any())
                        {
                            //var message = Encoding.UTF8.GetString(result.Buffer);
                            if (_receiveQueue.Count > instrument.MaxSendCount)
                            {
                                _receiveQueue.TryDequeue(out RecordMessageModel msg);
                                //todo:记录被剔除的消息
                                Log.LogInformation($"接收时间:{msg.RecordTime},内容为:{msg.RecordMessage}被剔除接收消息队列");
                            }

                            //_eventAggregator?.GetEvent<RecordMessageEvent>().Publish(new RecordMessageModel
                            //{
                            //    RecordTime = DateTime.Now,
                            //    RecordMessage = $"接收数据:{result.Buffer.ToAppendString()}"
                            //});
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }

            }, TaskCreationOptions.LongRunning);
        }
    }
}
