using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Domain.Instruments.Core.Enums
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
