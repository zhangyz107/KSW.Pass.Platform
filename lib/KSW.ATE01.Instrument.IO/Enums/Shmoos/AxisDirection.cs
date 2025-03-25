using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.Shmoos
{
    public enum AxisDirection
    {
        /// <summary>
        /// 一维
        /// </summary>
        [Description("None")]
        None,
        /// <summary>
        /// XY坐标轴
        /// </summary>
        [Description("X_Y")]
        XY,

        /// <summary>
        /// YX坐标轴
        /// </summary>
        [Description("Y_X")]
        YX
    }
}
