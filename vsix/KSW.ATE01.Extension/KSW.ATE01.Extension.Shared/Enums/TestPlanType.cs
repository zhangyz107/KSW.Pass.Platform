using System.ComponentModel;

namespace KSW.ATE01.Extension.Shared.Enums
{
    /// <summary>
    /// 测试计划类型
    /// </summary>
    public enum TestPlanType
    {
        /// <summary>
        /// Excel
        /// </summary>
        [Description("Excel")]
        Excel = 1,

        /// <summary>
        /// Csv
        /// </summary>
        [Description("Csv")]
        Csv = 2
    }
}
