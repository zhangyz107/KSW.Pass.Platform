using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.Loggers
{
    public enum DataLogFlag
    {
        /// <summary>
        /// 头或开始
        /// </summary>
        [Description("HeaderOrStart")]
        HeaderOrStart = 1,

        /// <summary>
        /// 内容
        /// </summary>
        [Description("Content")]
        Content = 3,

        /// <summary>
        /// 尾或结束
        /// </summary>
        [Description("EndOrStop")]
        EndOrStop = 4,
    }
}
