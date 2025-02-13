using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.Errors
{
    public enum OutputType
    {
        /// <summary>
        /// RealTimeTxt
        /// </summary>
        [Description("RealTimeTxt")]
        RealTimeTxt = 1,

        /// <summary>
        /// ErrorLog
        /// </summary>
        [Description("ErrorLog")]
        ErrorLog,

        /// <summary>
        /// CommonLog
        /// </summary>
        [Description("CommonLog")]
        CommonLog,

        /// <summary>
        /// Exception
        /// </summary>
        [Description("Exception")]
        Exception
    }
}
