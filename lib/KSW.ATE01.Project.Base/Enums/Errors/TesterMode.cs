using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.Errors
{
    /// <summary>
    /// 测试模式
    /// </summary>
    public enum TesterMode
    {
        /// <summary>
        /// Offline
        /// </summary>
        [Description("Offline")]
        Offline = 1,

        /// <summary>
        /// Online
        /// </summary>
        [Description("Online")]
        Online = 2,

        /// <summary>
        /// Error
        /// </summary>
        [Description("Error")]
        Error = 3,
    }
}
