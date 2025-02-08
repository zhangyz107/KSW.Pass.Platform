using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.TestPlans
{
    /// <summary>
    /// 测试项目参数模型
    /// </summary>
    public class TestItemParamModel
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string ParamValue { get; set; }
    }
}
