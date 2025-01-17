using KSW.Application;
using KSW.ATE01.Pattern.Domain.Instruments.Core.Enums;

namespace KSW.ATE01.Pattern.Application.BLLs.Abstractions.Instruments
{
    /// <summary>
    /// 设备连接操作工厂
    /// </summary>
    public interface IInstrumentControlFactory : IService
    {
        /// <summary>
        /// 注册控制服务
        /// </summary>
        void RegisterControlServices();

        /// <summary>
        /// 创建设备连接服务
        /// </summary>
        /// <param name="ioType"></param>
        /// <returns></returns>
        IInstruentControlService GetInstrumentControlService(IOTypeEnum ioType);
    }
}
