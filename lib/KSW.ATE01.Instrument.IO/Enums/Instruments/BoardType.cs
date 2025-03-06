using System.ComponentModel;

namespace KSW.ATE01.Instrument.IO.Enums.Instruments
{
    /// <summary>
    /// 板卡类型
    /// </summary>
    public enum BoardType
    {
        /// <summary>
        /// PE板卡
        /// </summary>
        [Description("PE")]
        PE = 0x0101,

        /// <summary>
        /// DPS板卡
        /// </summary>
        [Description("DPS")]
        DPS = 0x0201,

        /// <summary>
        /// 背板
        /// </summary>
        [Description("Backplane")]
        Backplane = 0x0301,
    }
}
