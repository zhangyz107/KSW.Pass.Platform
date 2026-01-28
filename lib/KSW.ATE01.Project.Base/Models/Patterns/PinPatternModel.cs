using KSW.ATE01.Project.Base.Enums.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Patterns
{
    /// <summary>
    /// 引脚Pattern模型
    /// </summary>
    public class PinPatternModel
    {
        /// <summary>
        /// 引脚名称
        /// </summary>
        public string PinName { get; set; }

        /// <summary>
        /// 时钟设置
        /// </summary>
        public string TimingSet { get; set; }

        /// <summary>
        /// 指令
        /// </summary>
        public CommandType Instruction { get; set; }

        /// <summary>
        /// 指令参数
        /// </summary>
        public object CommandParameter { get; set; }

        /// <summary>
        /// 向量值
        /// </summary>
        public VectorValueType VectorValue { get; set; }
    }
}
