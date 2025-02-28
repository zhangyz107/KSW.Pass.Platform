using KSW.ATE01.Project.Base.Enums.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Errors
{
    public class ErrorInfo
    {
        /// <summary>
        /// 模块名
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 是否报警
        /// </summary>
        public bool IsAlarm { get; set; }

        /// <summary>
        /// 错误名
        /// </summary>
        public string ErrorName { get; set; }

        /// <summary>
        /// 行为
        /// </summary>
        public BehaviorType Behavior { get; set; }

        /// <summary>
        /// 错误内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
