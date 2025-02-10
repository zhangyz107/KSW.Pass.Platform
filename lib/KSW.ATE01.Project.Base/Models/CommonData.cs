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
    }
}
