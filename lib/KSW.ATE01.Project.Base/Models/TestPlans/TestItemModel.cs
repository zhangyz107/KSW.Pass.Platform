using System.Collections.Generic;

namespace KSW.ATE01.Project.Base.Models.TestPlans
{
    /// <summary>
    /// 测试项模型
    /// </summary>
    public class TestItemModel
    {
        /// <summary>
        /// 标识符
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Sheet名称
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// 测试项名称
        /// </summary>
        public string TestItemName { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// 强制值
        /// </summary>
        public decimal Force { get; set; }

        /// <summary>
        /// 待测引脚
        /// </summary>
        public string Pins { get; set; }

        /// <summary>
        /// 标准Sheet名
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 电压参数集合
        /// </summary>
        public List<LevelModel> Levels { get; set; } = new List<LevelModel>();

        /// <summary>
        /// 时钟Sheet名
        /// </summary>
        public string Timing { get; set; }

        /// <summary>
        /// 时钟参数集合
        /// </summary>
        public List<TimingModel> Timings { get; set; } = new List<TimingModel>();

        /// <summary>
        /// 参数
        /// </summary>
        public List<TestItemParamModel> Args { get; set; } = new List<TestItemParamModel>();
    }
}
