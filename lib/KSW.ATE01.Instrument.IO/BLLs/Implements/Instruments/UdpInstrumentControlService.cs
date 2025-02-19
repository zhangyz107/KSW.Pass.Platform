/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：UdpInstrumentControlService.cs
// 功能描述：Udp设备控制服务
//
// 作者：zhangyingzhong
// 日期：2025/01/21 18:03
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Project.Base.Extensions;
using KSW.ATE01.Project.Base.Helpers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements
{
    /// <summary>
    /// Udp设备控制服务
    /// </summary>
    public class UdpInstrumentControlService : InstrumentControlBaseService
    {
        #region Field
        private ConcurrentDictionary<string, UdpClient> _connectionPool = new ConcurrentDictionary<string, UdpClient>();

        //发送队列
        private readonly ConcurrentQueue<SendMessageModel> _sendQueue = new ConcurrentQueue<SendMessageModel>();
        private readonly ConcurrentQueue<RecordMessageModel> _receiveQueue = new ConcurrentQueue<RecordMessageModel>();

        //发送队列最多支持命令数量
        private readonly int _maxSendCount = 100;
        private readonly int _bufferSize = 8192;
        #endregion

        #region Properties
        public override event Action<SendMessageModel> SendMessageEvent;
        public override event Action<RecordMessageModel> ReceiveMessageEvent;
        #endregion

        public UdpInstrumentControlService()
        {
            StartSendQueueTask();
        }

        public override IInstruentControlService CreateConnect(InstrumentBaseModel instrument)
        {
            try
            {
                if (string.IsNullOrEmpty(instrument.Address))
                    throw new ArgumentNullException(nameof(InstrumentBaseModel.Address), "设备地址不能为空");

                if (_connectionPool.Keys.Contains(instrument.Address))
                    return this;

                if (PraseIPAddress(instrument.Address, out string ipAddress, out int port, out int localPort))
                {
                    var tempInstrumentInfo = DeepCopyHelper.Copy(instrument);
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

        public override IInstruentControlService DestroyConnect(InstrumentBaseModel instrument)
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

        public override bool IsConnected(InstrumentBaseModel instrument)
        {
            return _connectionPool.Keys.Contains(instrument.Address);
        }

        public override void Send(InstrumentBaseModel instrument, byte[] data)
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

        public override byte[] Query(InstrumentBaseModel instrument, byte[] data)
        {
            var receiveData = new byte[_bufferSize];
            try
            {
                var udpClient = _connectionPool[instrument.Address];
                if (udpClient == null)
                    throw new ArgumentNullException(nameof(instrument.Address), "无法发送到空设备 (检查设备是否正常连接)！");

                udpClient.Client.SendTimeout = instrument.SendTimeOut;
                udpClient.Client.Send(data);

                var length = udpClient.Client.Receive(receiveData);
                return receiveData.AsSpan().Slice(0, length).ToArray();
            }
            catch
            {
                throw;
            }
        }

        public override void Send(InstrumentBaseModel instrument, string data)
        {
            if (string.IsNullOrEmpty(data)) { return; }

            if (_sendQueue.Count > _maxSendCount)
            {
                _sendQueue.TryDequeue(out SendMessageModel message);
                //todo:记录被剔除的消息
                //Log.LogInformation($"向设备{message.Address}发送内容:{message.StringEncoder.GetString(message.Message)}被剔除发送队列");
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

        public override byte[] Query(InstrumentBaseModel instrument, string data)
        {
            var receiveData = new byte[_bufferSize];
            try
            {
                var udpClient = _connectionPool[instrument.Address];
                if (udpClient == null)
                    throw new ArgumentNullException(nameof(instrument.Address), "无法发送到空设备 (检查设备是否正常连接)！");

                var dataBytes = instrument.StringEncoder.GetBytes(data);

                udpClient.Client.Send(dataBytes);

                var length = udpClient.Client.Receive(receiveData);
                return receiveData.AsSpan().Slice(0, length).ToArray();
            }
            catch
            {
                throw;
            }
        }

        public override void SendLine(InstrumentBaseModel instrument, string data)
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

        protected override bool ConnectInstrument(InstrumentBaseModel instrument)
        {
            var isConnected = false;
            try
            {
                if (string.IsNullOrEmpty(instrument.IpAddress))
                    throw new ArgumentNullException(nameof(InstrumentBaseModel.IpAddress), "当前设备地址为空!");

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

                udpClient.Client.SendTimeout = instrument.SendTimeOut;
                udpClient.Client.ReceiveTimeout = instrument.ReceiveTimeOut;
                isConnected = true;

                _connectionPool.TryAdd(instrument.Address, udpClient);
                //StartReceiveTask(instrument);
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
                        //Log.LogError(e, e.Message);
                    }
                }

            }, TaskCreationOptions.LongRunning);
        }

        private void SendToInstrument(string address, byte[] data, int timeOut, bool hasAck = true)
        {
            try
            {
                var receiveData = new byte[_bufferSize];
                var udpClient = _connectionPool[address];
                if (udpClient == null)
                    throw new ArgumentNullException(nameof(address), "无法发送到空设备 (检查设备是否正常连接)！");

                udpClient.Client.Send(data);

                SendMessageEvent?.Invoke(new SendMessageModel { Address = address, Message = data });

                if (hasAck)
                {
                    var size = udpClient.Client.Receive(receiveData);
                    if (size == data.Length)
                    {
                        Debug.WriteLine("数据发送成功！");
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void StartReceiveTask(InstrumentBaseModel instrument)
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
                            var message = new RecordMessageModel
                            {
                                RecordTime = DateTime.Now,
                                OriginalData = result.Buffer
                            };

                            if (_receiveQueue.Count > instrument.MaxSendCount)
                            {
                                _receiveQueue.TryDequeue(out RecordMessageModel msg);
                                //todo:记录被剔除的消息
                                //Log.LogInformation($"接收时间:{msg.RecordTime},内容为:{msg.RecordMessage}被剔除接收消息队列");
                            }
                            else
                            {
                                _receiveQueue.Enqueue(message);
                            }

                            ReceiveMessageEvent?.Invoke(message);
                        }
                        else
                        {
                            await Task.Delay(1);
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
