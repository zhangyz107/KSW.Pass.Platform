using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.Patterns
{
    /// <summary>
    /// 引脚类型
    /// </summary>
    public enum PinIOType
    {
        /// <summary>
        /// input
        /// </summary>
        [Description("input")]
        input = 0,

        /// <summary>
        /// output
        /// </summary>
        [Description("output")]
        output = 1,

        /// <summary>
        /// inout
        /// </summary>
        [Description("inout")]
        inout = 2,
    }
}
