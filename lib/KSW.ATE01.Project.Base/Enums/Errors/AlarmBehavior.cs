using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.Errors
{
    public enum AlarmBehavior
    {
        /// <summary>
        /// Ignore
        /// </summary>
        [Description("Ignore")]
        Ignore = 1,

        /// <summary>
        /// ForceFail
        /// </summary>
        [Description("ForceFail")]
        ForceFail = 2,
    }
}
