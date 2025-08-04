using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Drawing.Shmoo.IO.Enums.Shmoos
{
    /// <summary>
    /// Shmoo结果类型
    /// </summary>
    public enum ShmooResultType
    {
        /// <summary>
        /// 稳定区
        /// </summary>
        Stability = 0,

        /// <summary>
        /// 失败区
        /// </summary>
        Failure = 80,

        /// <summary>
        /// 过渡区
        /// </summary>
        Transition1 = 20,

        /// <summary>
        /// 过渡区
        /// </summary>
        Transition2 = 40,

        /// <summary>
        /// 极限稳定区
        /// </summary>
        ExtremeStability = 60,

        /// <summary>
        /// 未测试区域
        /// </summary>
        Untested = 100
    }
}
