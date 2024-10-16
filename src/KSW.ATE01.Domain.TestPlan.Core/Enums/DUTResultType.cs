using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Domain.TestPlan.Core.Enums
{
    /// <summary>
    /// 测试结果类型
    /// </summary>
    public enum DUTResultType
    {
        /// <summary>
        /// Pass
        /// </summary>
        [Description("Pass")]
        Pass = 1,

        /// <summary>
        /// Fail
        /// </summary>
        [Description("Fail")]
        Fail = 2,

        /// <summary>
        /// Error
        /// </summary>
        [Description("Error")]
        Error = 3
    }
}
