using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.ALPG
{
    public enum WritePostamble
    {
        /// <summary>
        /// 0.5 tCK-0 Pattern
        /// </summary>
        [Description("0.5 tCK-0 Pattern")]
        tCKpoint5,

        /// <summary>
        /// 1.5 tCK-010 Pattern
        /// </summary>
        [Description("1.5 tCK-000 Pattern")]
        tCK1point5
    }
}
