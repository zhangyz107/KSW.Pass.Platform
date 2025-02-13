using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.Errors
{
    public enum ErrorStatus
    {
        /// <summary>
        /// Normal
        /// </summary>
        [Description("Normal")]
        Normal = 1,

        /// <summary>
        /// Warning
        /// </summary>
        [Description("Warning")]
        Warning = 2,

        /// <summary>
        /// Error
        /// </summary>
        [Description("Error")]
        Error = 3,

        /// <summary>
        /// Fatal
        /// </summary>
        [Description("Fatal")]
        Fatal = 4,

        /// <summary>
        /// Unkown
        /// </summary>
        [Description("Unkown")]
        Unkown = 255
    }
}
