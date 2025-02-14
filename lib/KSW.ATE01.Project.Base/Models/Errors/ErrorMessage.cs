using KSW.ATE01.Project.Base.Enums.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Errors
{
    public class ErrorMessage
    {
        /// <summary>
        /// 模块名
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 遇到异常后行为
        /// </summary>
        public BehaviorType Behavior { get; set; }

        /// <summary>
        /// 错误名
        /// </summary>
        public string ErrorName { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        public uint Number { get; set; }

        /// <summary>
        /// 错误
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Location { get; set; }
    }
}
