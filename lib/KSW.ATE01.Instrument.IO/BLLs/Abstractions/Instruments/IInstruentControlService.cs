using KSW.ATE01.Instrument.IO.Models.Instruments;

namespace KSW.ATE01.Instrument.IO
{
    /// <summary>
    /// 设备控制服务接口
    /// </summary>
    public interface IInstruentControlService
    {
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        IInstruentControlService CreateConnect(InstrumentBaseModel instrument);

        /// <summary>
        /// 是否连接
        /// </summary>
        bool IsConnected(InstrumentBaseModel instrument);

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        IInstruentControlService DestroyConnect(InstrumentBaseModel instrument);

        /// <summary>
        /// 发送数据
        /// </summary>
        void Send(InstrumentBaseModel instrument, byte[] data, bool direct = true, bool hasAck = true);

        /// <summary>
        /// 查询数据
        /// </summary>
        byte[] Query(InstrumentBaseModel instrument, byte[] data);

        /// <summary>
        /// 发送数据
        /// </summary>
        void Send(InstrumentBaseModel instrument, string data, bool direct = true, bool hasAck = true);

        /// <summary>
        /// 查询数据
        /// </summary>
        byte[] Query(InstrumentBaseModel instrument, string data);

        /// <summary>
        /// 发送数据
        /// </summary>
        void SendLine(InstrumentBaseModel instrument, string data, bool direct = true, bool hasAck = true);
    }
}
