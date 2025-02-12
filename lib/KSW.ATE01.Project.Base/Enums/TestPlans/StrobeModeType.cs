using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.TestPlans
{
    public enum StrobeModeType
    {
        /// <summary>
        /// OFF
        /// </summary>
        [Description("OFF")]
        OFF = 0,

        /// <summary>
        /// Edge
        /// </summary>
        [Description("Edge")]
        Edge = 1
    }
}
