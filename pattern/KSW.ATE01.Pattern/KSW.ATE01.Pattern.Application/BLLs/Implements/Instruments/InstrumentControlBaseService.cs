using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Instruments;
using KSW.ATE01.Pattern.Application.Models.Instruments;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace KSW.ATE01.Pattern.Application.BLLs.Implements.Instruments
{
    /// <summary>
    /// 设备控制基类
    /// </summary>
    public abstract class InstrumentControlBaseService : IInstruentControlService
    {
        #region Properties
        /// <summary>
        /// 容器
        /// </summary>
        protected IContainerProvider ContainerProvider { get; }

        /// <summary>
        /// 日志记录
        /// </summary>
        protected ILogger Log { get; }
        #endregion

        protected InstrumentControlBaseService(IContainerProvider containerProvider)
        {
            ContainerProvider = containerProvider;
            var logFactory = containerProvider.Resolve<ILoggerFactory>() ?? throw new ArgumentNullException(nameof(ILoggerFactory));
            Log = logFactory?.CreateLogger(GetType());
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        public abstract IInstruentControlService CreateConnect(InstrumentInfoModel instrument);

        public abstract bool IsConnected(InstrumentInfoModel instrument);
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public abstract IInstruentControlService DestroyConnect(InstrumentInfoModel instrument);

        /// <summary>
        /// 发送数据
        /// </summary>
        public abstract void Send(InstrumentInfoModel instrument, byte[] data);

        /// <summary>
        /// 发送数据
        /// </summary>
        public abstract void Send(InstrumentInfoModel instrument, string data);

        /// <summary>
        /// 发送数据
        /// </summary>
        public abstract void SendLine(InstrumentInfoModel instrument, string data);
        /// <summary>
        /// 连接设备
        /// </summary>
        /// <returns></returns>
        protected abstract bool ConnectInstrument(InstrumentInfoModel instrument);

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
            PingReply reply = pingtest.Send(ipAddress, 1000, buff, myOptions);
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
