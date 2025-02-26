using KSW.ATE01.Instrument.IO.Enums.Patterns;
using KSW.ATE01.Instrument.IO.Models.Results;

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
        //void SetPatternParam();

#if DEBUG
        /// <summary>
        /// Pattern运行或停止
        /// </summary>
        void SetPatternEnable(bool enable);

        /// <summary>
        /// 获取Pattern运行使能
        /// </summary>
        ChannelResultModel<bool> GetPatternEnable();
#endif

        /// <summary>
        /// 获取运行状态
        /// </summary>
        List<PatternRunningStateModel> GetRunningState();

        /// <summary>
        /// 获取首次错误位置
        /// </summary>
        List<ChannelResultModel<int>> GetFailPosition();

        /// <summary>
        /// 获取向量结果数据存储地址
        /// </summary>
        List<ChannelResultModel<PatternStorageAddress>> GetStorageAddress();

        /// <summary>
        /// 获取运行结果
        /// </summary>
        /// <param name="startAddress">起始地址</param>
        /// <param name="length">数据长度</param>
        List<ChannelResultModel<PatternResultModel>> GetResult(ushort startAddress, short length);
    }
}
