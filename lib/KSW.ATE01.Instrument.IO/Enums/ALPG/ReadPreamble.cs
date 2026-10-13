using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.ALPG
{
    public enum ReadPreamble
    {
        /// <summary>
        /// 1tCK-10 Pattern
        /// </summary>
        [Description("1tCK-10 Pattern")]
        tCK1,

        /// <summary>
        /// 2tCK-0010Pattern
        /// </summary>
        [Description("2tCK-0010Pattern")]
        tCK2_10Pattern,

        /// <summary>
        /// 2tCK-1110Pattern
        /// </summary>
        [Description("2tCK-1110Pattern")]
        tCK2_1110Pattern,

        /// <summary>
        /// 3tCK-000010Pattern
        /// </summary>
        [Description("3tCK-000010Pattern")]
        tCK3,

        /// <summary>
        /// 4tCK-00001010Pattern
        /// </summary>
        [Description("4tCK-00001010Pattern")]
        tCK4
    }
}
