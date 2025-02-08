using KSW.ATE01.Instrument.IO.Enums.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO
{
    /// <summary>
    /// 设备连接操作工厂
    /// </summary>
    public interface IInstrumentControlFactory
    {
        /// <summary>
        /// 注册控制服务
        /// </summary>
        void RegisterControls();

        /// <summary>
        /// 创建设备连接服务
        /// </summary>
        /// <param name="ioType"></param>
        /// <returns></returns>
        IInstruentControlService? GetInstrumentControlService(IOTypeEnum ioType);
    }
}
