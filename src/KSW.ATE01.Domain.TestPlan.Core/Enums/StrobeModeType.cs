using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Domain.TestPlan.Core.Enums
{
    public enum StrobeModeType
    {
        /// <summary>
        /// OFF
        /// </summary>
        [Description("OFF")]
        OFF = 1,

        /// <summary>
        /// Edge
        /// </summary>
        [Description("Edge")]
        Edge = 2
    }
}
