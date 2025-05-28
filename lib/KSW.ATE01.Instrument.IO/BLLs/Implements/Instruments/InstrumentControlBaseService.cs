using KSW.ATE01.Instrument.IO.Models.Instruments;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements
{
    /// <summary>
    /// 设备控制基类
    /// </summary>
    public abstract class InstrumentControlBaseService : IInstruentControlService
    {
        #region Properties
        public abstract event Action<SendMessageModel> SendMessageEvent;

        public abstract event Action<RecordMessageModel> ReceiveMessageEvent;
        #endregion

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        public abstract IInstruentControlService CreateConnect(InstrumentBaseModel instrument);

        /// <summary>
        /// 测试连接
        /// </summary>
        public bool TestConnect(string ipAddress)
        {
            return IsHostOnline(IPAddress.Parse(ipAddress));
        }

        /// <summary>
        /// 是否连接
        /// </summary>
        public abstract bool IsConnected(InstrumentBaseModel instrument);

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public abstract IInstruentControlService DestroyConnect(InstrumentBaseModel instrument);

        /// <summary>
        /// 通过IP地址发送数据
        /// </summary>
        public abstract void Send(string ipAddress, int port, byte[] data, out int localPort, bool hasAck = true);

        /// <summary>
        /// 通过IP地址查询数据
        /// </summary>
        public abstract byte[] Query(string ipAddress, int port, byte[] data, out int localPort);

        /// <summary>
        /// 发送数据
        /// </summary>
        public abstract void Send(InstrumentBaseModel instrument, byte[] data, bool direct = true, bool hasAck = true);

        /// <summary>
        /// 查询数据
        /// </summary>
        public abstract byte[] Query(InstrumentBaseModel instrument, byte[] data);

        /// <summary>
        /// 发送数据
        /// </summary>
        public abstract void Send(InstrumentBaseModel instrument, string data, bool direct = true, bool hasAck = true);

        /// <summary>
        /// 查询数据
        /// </summary>
        public abstract byte[] Query(InstrumentBaseModel instrument, string data);

        /// <summary>
        /// 发送数据
        /// </summary>
        public abstract void SendLine(InstrumentBaseModel instrument, string data, bool direct = true, bool hasAck = true);
        /// <summary>
        /// 连接设备
        /// </summary>
        /// <returns></returns>
        protected abstract bool ConnectInstrument(InstrumentBaseModel instrument);

        /// <summary>
        /// 将设备地址转换成Ip地址、端口号、本机端口号
        /// </summary>
        protected virtual bool PraseIPAddress(string address, out string ipAddress, out int port, out int localPort)
        {
            string[] addrarray = address.Split(':');
            ipAddress = "";
            port = 0;
            localPort = 0;
            if (addrarray.Length >= 3)
            {
                ipAddress = addrarray[1];
                port = Convert.ToInt32(addrarray[2]);
                localPort = Convert.ToInt32(addrarray[3]);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        protected virtual bool IsHostOnline(IPAddress ipAddress)
        {
            Ping pingtest = new Ping();
            PingOptions myOptions = new PingOptions();
            myOptions.DontFragment = true;
            string data = "test";
            byte[] buff = Encoding.ASCII.GetBytes(data);
            PingReply reply = pingtest.Send(ipAddress, 100, buff, myOptions);
            if (reply.Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
