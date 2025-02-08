using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.TestPlans
{
    /// <summary>
    /// 全局模型
    /// </summary>
    public class GlobalModel
    {
        /// <summary>
        /// 标识符
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Pattern文件名
        /// </summary>
        public string PatternFile { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public List<GlobalParamModel> Args { get; set; } = new List<GlobalParamModel>();
    }
}
