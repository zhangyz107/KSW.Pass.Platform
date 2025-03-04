using System.ComponentModel;

namespace KSW.ATE01.DPS.Domain.Core.Enums
{
    public enum NodeLineShape
    {
        /// <summary>
        /// 直线
        /// </summary>
        [Description("Line")]
        Line = 0,

        /// <summary>
        /// 波浪线
        /// </summary>
        [Description("WaveLine")]
        WaveLine = 1,
    }
}
