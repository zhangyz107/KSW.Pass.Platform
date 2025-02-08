using System.ComponentModel;

namespace KSW.ATE01.Instrument.IO.Enums.Instruments
{
    /// <summary>
    /// 指令类型
    /// </summary>
    public enum InstructionType
    {
        /// <summary>
        /// 配置
        /// </summary>
        [Description("0x01")]
        Configuration = 1,

        /// <summary>
        /// 查询
        /// </summary>
        [Description("0x02")]
        Query = 2,

        /// <summary>
        /// 配置失败
        /// </summary>
        [Description("0x03")]
        ConfigurationFailed = 3,

        /// <summary>
        /// 配置成功
        /// </summary>
        [Description("0x04")]
        ConfigurationSuccessful = 4,

        /// <summary>
        /// 查询失败
        /// </summary>
        [Description("0x05")]
        QueryFailed = 5,

        /// <summary>
        /// 查询成功
        /// </summary>
        [Description("0x06")]
        QuerySuccessful = 6
    }
}
