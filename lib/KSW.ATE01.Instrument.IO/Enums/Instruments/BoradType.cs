using System.ComponentModel;

namespace KSW.ATE01.Instrument.IO.Enums.Instruments
{
    /// <summary>
    /// 板卡类型
    /// </summary>
    public enum BoradType
    {
        /// <summary>
        /// PE板卡
        /// </summary>
        [Description("0x0101")]
        PE = 1,

        /// <summary>
        /// DPS板卡
        /// </summary>
        [Description("0x0201")]
        DPS = 2,
    }
}
