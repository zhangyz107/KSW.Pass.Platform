using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.Shmoos
{
    /// <summary>
    /// 坐标轴类型
    /// </summary>
    public enum AxisType
    {
        /// <summary>
        /// 水平 
        /// </summary>
        [Description("Level")]
        Level,

        /// <summary>
        /// 时钟
        /// </summary>
        [Description("Timing")]
        Timing,

        /// <summary>
        /// 电压
        /// </summary>
        [Description("Power")]
        Power
    }
}
