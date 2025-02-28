using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Domain.Projects.Core.Enums
{
    public enum ProcessStage
    {
        /// <summary>
        /// 测试项
        /// </summary>
        [Description("TestItem")]
        TestItem = 0,

        /// <summary>
        /// 测试开始
        /// </summary>
        [Description("TestStart")]
        TestStart = 1,

        /// <summary>
        /// 测试结束
        /// </summary>
        [Description("TestEnd")]
        TestEnd =2,

        /// <summary>
        /// 流程开始
        /// </summary>
        [Description("FlowStart")]
        FlowStart = 3,

        /// <summary>
        /// 流程结束
        /// </summary>
        [Description("FlowEnd")]
        FlowEnd = 4,

    }
}
