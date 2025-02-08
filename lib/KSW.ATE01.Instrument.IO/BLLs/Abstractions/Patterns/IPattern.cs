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
        /// 设置周期
        /// </summary>
        /// <param name="patternPeriod">向量周期(0~0.02684354559375)</param>
        /// <param name="waveformType">波形</param>
        /// <param name="strobeType">边沿类型</param>
        /// <param name="d0">D0位置(0~ Period)</param>
        /// <param name="d1">D1位置(0~ Period)</param>
        /// <param name="d2">D2位置(0~ Period)</param>
        /// <param name="d3">D3位置(0~ Period)</param>
        /// <param name="r0">R0位置(0~ Period)</param>
        /// <param name="r1">R1位置(0~ Period)</param>
        /// <param name="pwa_en">-128~+127</param>
        /// <param name="cd_en">0~63</param>
        /// <param name="fd_en">0~63</param>
        /// <param name="pwa_d">-128~+127</param>
        /// <param name="cd_d">0~63</param>
        /// <param name="fd_d">0~63</param>
        /// <param name="pwa_ca">-128~+127</param>
        /// <param name="cd_ca">0~63</param>
        /// <param name="fd_ca">0~63</param>
        /// <param name="pwa_cb">-128~+127</param>
        /// <param name="cd_cb">0~63</param>
        /// <param name="fd_cb">0~63</param>
        void SetTiming(
            double patternPeriod,
            WaveformType waveformType,
            StrobeType strobeType,
            double d0,
            double d1,
            double d2,
            double d3,
            double r0,
            double r1,
            sbyte pwa_en,
            byte cd_en,
            ushort fd_en,
            sbyte pwa_d,
            byte cd_d,
            ushort fd_d,
            sbyte pwa_ca,
            byte cd_ca,
            ushort fd_ca,
            sbyte pwa_cb,
            byte cd_cb,
            ushort fd_cb
            );

        /// <summary>
        /// 设置Pattern参数
        /// </summary>
        void SetPatternParam();
    }
}
