using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Domain.Projects.Core.Enums
{
    /// <summary>
    /// 波形类型
    /// </summary>
    public enum WaveformType
    {
        /// <summary>
        /// NR
        /// </summary>
        [Description("NR")]
        NR = 0,

        /// <summary>
        /// RL
        /// </summary>
        [Description("RL")]
        RL = 1,

        /// <summary>
        /// RH
        /// </summary>
        [Description("RH")]
        RH = 2,

        /// <summary>
        /// SBC
        /// </summary>
        [Description("SBC")]
        SBC = 3,
    }
}
