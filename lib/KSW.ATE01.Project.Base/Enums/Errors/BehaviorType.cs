using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.Errors
{
    public enum BehaviorType
    {
        /// <summary>
        /// None
        /// </summary>
        [Description("None")]
        None = 0,

        ///// <summary>
        ///// ForceFail
        ///// </summary>
        //[Description("ForceFail")]
        //ForceFail = 1,

        ///// <summary>
        ///// ForceBin
        ///// </summary>
        //[Description("ForceBin")]
        //ForceBin = 2,

        ///// <summary>
        ///// ForceHalt
        ///// </summary>
        //[Description("ForceHalt")]
        //ForceHalt = 3,

        ///// <summary>
        ///// ForceReset
        ///// </summary>
        //[Description("ForceReset")]
        //ForceReset = 4,

        /// <summary>
        /// Off
        /// </summary>
        [Description("Off")]
        Off = 5,

        /// <summary>
        /// Default
        /// </summary>
        [Description("Default")]
        Default = 6,

        /// <summary>
        /// Continue
        /// </summary>
        [Description("Continue")]
        Continue = 7,
    }
}
