using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.TestPlans
{
    /// <summary>
    /// 测试计划Sheet类型
    /// </summary>
    public enum TestPlanSheetType
    {
        /// <summary>
        /// 通道
        /// </summary>
        [Description("Channel")]
        Channel = 1,

        /// <summary>
        /// 测试项
        /// </summary>
        [Description("TestItem")]
        TestItem = 2,

        /// <summary>
        /// 限制
        /// </summary>
        [Description("Limits")]
        Limits = 3,

        /// <summary>
        /// 计划
        /// </summary>
        [Description("Flow")]
        Flow = 4,

        /// <summary>
        /// 标准
        /// </summary>
        [Description("Level")]
        Level = 5,

        /// <summary>
        /// 时钟
        /// </summary>
        [Description("Timing")]
        Timing = 6,

        /// <summary>
        /// 全局数据
        /// </summary>
        [Description("Global")]
        Global = 7,
    }
}
