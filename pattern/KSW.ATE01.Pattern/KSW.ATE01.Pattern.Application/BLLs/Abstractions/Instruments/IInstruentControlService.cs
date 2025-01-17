using KSW.ATE01.Pattern.Application.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.BLLs.Abstractions.Instruments
{
    public interface IInstruentControlService
    {
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        IInstruentControlService CreateConnect(InstrumentInfoModel instrument);

        /// <summary>
        /// 是否连接
        /// </summary>
        bool IsConnected(InstrumentInfoModel instrument);

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        IInstruentControlService DestroyConnect(InstrumentInfoModel instrument);

        /// <summary>
        /// 发送数据
        /// </summary>
        void Send(InstrumentInfoModel instrument, byte[] data);

        /// <summary>
        /// 发送数据
        /// </summary>
        void Send(InstrumentInfoModel instrument, string data);

        /// <summary>
        /// 发送数据
        /// </summary>
        void SendLine(InstrumentInfoModel instrument, string data);
    }
}
