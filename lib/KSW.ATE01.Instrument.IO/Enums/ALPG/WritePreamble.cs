using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.ALPG
{
    public enum WritePreamble
    {
        /// <summary>
        /// 2tCK-0010 Pattern
        /// </summary>
        [Description("2tCK-0010 Pattern")]
        tCK2 = 1,

        /// <summary>
        /// 3tCK-000010 Pattern
        /// </summary>
        [Description("3tCK-000010 Pattern")]
        tCK3 = 2,

        /// <summary>
        /// 4tCK-00001010 Pattern
        /// </summary>
        [Description("4tCK-00001010 Pattern")]
        tCK4 = 3
    }
}
