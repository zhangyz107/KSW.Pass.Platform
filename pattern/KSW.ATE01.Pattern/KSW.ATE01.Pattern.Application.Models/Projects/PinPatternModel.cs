using KSW.ATE01.Pattern.Domain.Projects.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Models.Projects
{
    public struct PinPatternModel
    {
        /// <summary>
        /// 引脚名称
        /// </summary>
        public string PinName;

        /// <summary>
        /// 时钟设置
        /// </summary>
        public string TimingSet;

        /// <summary>
        /// 指令
        /// </summary>
        public CommandType Instruction;

        /// <summary>
        /// 指令参数
        /// </summary>
        public object CommandParameter;

        /// <summary>
        /// 向量值
        /// </summary>
        public VectorValueType VectorValue;
    }
}
