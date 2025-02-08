using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.TestPlans
{
    /// <summary>
    /// 通道类型
    /// </summary>
    public enum ChannelType
    {
        /// <summary>
        /// I/O
        /// </summary>
        [Description("I/O")]
        IO = 1,

        /// <summary>
        /// DPS
        /// </summary>
        [Description("DPS")]
        DPS = 2,

        /// <summary>
        /// VNA
        /// </summary>
        [Description("VNA")]
        VNA = 3,
    }
}
