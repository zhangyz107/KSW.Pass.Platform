using KSW.ATE01.Project.Base.Models.Projects;
using KSW.ATE01.Project.Base.Models.TestPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models
{
    /// <summary>
    /// 通用数据
    /// </summary>
    public class CommonData : MarshalByRefObject
    {
        private static object _lock = new object();
        private static CommonData _instance;

        private CommonData() { }

        public static CommonData Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CommonData();
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// 项目信息
        /// </summary>
        public ProjectInfo ProjectInfo { get; set; }

        /// <summary>
        /// 测试计划
        /// </summary>
        public TestPlanModel TestPlan { get; set; }

        /// <summary>
        /// 将要执行的方法名
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// 激励
        /// </summary>
        public double Force { get; set; }

        /// <summary>
        /// 引脚名
        /// </summary>
        public string Pins { get; set; }

        /// <summary>
        /// 将要执行的测试项
        /// </summary>
        public string TestItemName { get; set; }

        /// <summary>
        /// 测试计划Timing Sheet名
        /// </summary>
        public string Timing { get; set; }

        /// <summary>
        /// 测试计划Level Sheet名
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 测试项参数
        /// </summary>
        public List<TestItemParamModel> TestItemArgs { get; set; }

        /// <summary>
        /// 测试项门限
        /// </summary>
        public LimitsModel TestItemLimit { get; set; }

        /// <summary>
        /// 电压下限
        /// </summary>
        public decimal? LowLimit { get => TestItemLimit?.LowLimit; }

        /// <summary>
        /// 电压上限
        /// </summary>
        public decimal? HighLimit { get => TestItemLimit?.HighLimit; }
    }
}
