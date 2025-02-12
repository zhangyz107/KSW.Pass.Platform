using KSW.ATE01.Instrument.IO.Enums.Patterns;

namespace KSW.ATE01.Instrument.IO.BLLs.Abstractions.Patterns
{
    public interface IPattern
    {
        /// <summary>
        /// 设置引脚类型
        /// </summary>
        /// <param name="pinIOType">引脚类型</param>
        void SetPinType(PinIOType pinIOType);

        /// <summary>
        /// 设置引脚电压初始化类型
        /// </summary>
        void SetPinInit(PinInitVoltageType pinInitVoltageType);


        /// <summary>
        /// 设置Pattern参数
        /// </summary>
        void SetPatternParam();
    }
}
