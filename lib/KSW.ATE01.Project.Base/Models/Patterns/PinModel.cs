using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.ATE01.Project.Base.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Patterns
{
    /// <summary>
    /// 引脚模型
    /// </summary>
    public class PinModel
    {
        /// <summary>
        /// 引脚名
        /// </summary>
        public string PinName { get; set; }

        /// <summary>
        /// 引脚值
        /// </summary>
        public VectorValueType VectorValue { get; set; }

        public string VectorValueDescription => VectorValue.GetDescription();
    }
}
