using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.Results
{
    /// <summary>
    /// 测试结果
    /// </summary>
    public enum Test
    {
        /// <summary>
        /// 通过
        /// </summary>
        [Description("Pass")]
        Pass = 1,

        /// <summary>
        /// 失败
        /// </summary>
        [Description("Fail")]
        Fail = 2,

        /// <summary>
        /// 错误
        /// </summary>
        [Description("Error")]
        Error = 3,
    }
}
