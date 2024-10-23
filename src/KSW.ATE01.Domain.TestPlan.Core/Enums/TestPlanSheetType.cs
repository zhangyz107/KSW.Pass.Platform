using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Domain.TestPlan.Core.Enums
{
    /// <summary>
    /// 测试计划表类型
    /// </summary>
    public enum TestPlanSheetType
    {
        [Description("Channel")]
        Channel = 1,

        [Description("TestItem")]
        TestItem = 2,

        [Description("Limits")]
        Limits = 3,

        [Description("Flow")]
        Flow = 4,

        [Description("Level")]
        Level = 5,

        [Description("Timing")]
        Timing = 6,
    }
}
