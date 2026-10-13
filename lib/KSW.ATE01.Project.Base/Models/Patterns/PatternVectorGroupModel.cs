using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Patterns
{
    /// <summary>
    /// 向量组模型（512bit）
    /// </summary>
    public struct PatternVectorGroupModel
    {
        /// <summary>
        /// 引脚名称
        /// </summary>
        public string PinName;

        /// <summary>
        /// 向量数
        /// </summary>
        public byte VectorNumber;

        /// <summary>
        /// 指令+向量数最高位
        /// </summary>
        public byte Instruction;

        public PatternVectorGroupModel()
        {
            Data = new byte[64];  // 08.25取消指令Id    62
        }

        /// <summary>
        /// 操作数(包含操作数6byte+56byte向量)
        /// </summary>
        public byte[] Data;
    }
}
