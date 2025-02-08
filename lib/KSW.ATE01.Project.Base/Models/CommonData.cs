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
    public static class CommonData
    {
        /// <summary>
        /// 项目信息
        /// </summary>
        public static ProjectInfo ProjectInfo { get; set; }

        /// <summary>
        /// 测试计划
        /// </summary>
        public static TestPlanModel TestPlan { get; set; }
    }
}
