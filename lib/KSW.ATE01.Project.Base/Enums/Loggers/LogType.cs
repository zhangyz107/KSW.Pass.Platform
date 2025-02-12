using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.Loggers
{
    /// <summary>
    /// 记录类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Log
        /// </summary>
        [Description("Log")]
        Log = 0,

        /// <summary>
        /// LogWithPath
        /// </summary>
        [Description("LogWithPath")]
        LogWithPath = 1,

        /// <summary>
        /// Error
        /// </summary>
        [Description("Error")]
        Error = 2,

        /// <summary>
        /// ErrorWithPath
        /// </summary>
        [Description("ErrorWithPath")]
        ErrorWithPath = 3,

        /// <summary>
        /// Clear
        /// </summary>
        [Description("Clear")]
        Clear = 4,
    }
}
