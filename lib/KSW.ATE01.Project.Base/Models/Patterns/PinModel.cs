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
    public struct PinModel
    {
        /// <summary>
        /// 引脚名
        /// </summary>
        public string PinName;

        /// <summary>
        /// 引脚值
        /// </summary>
        public VectorValueType VectorValue;

        public string VectorValueDescription => VectorValue.GetDescription();
    }
}
