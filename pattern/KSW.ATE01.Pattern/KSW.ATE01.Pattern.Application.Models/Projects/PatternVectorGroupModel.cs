using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Models.Projects
{
    /// <summary>
    /// 向量组模型（512bit）
    /// </summary>
    public struct PatternVectorGroupModel
    {
        /// <summary>
        /// 向量数
        /// </summary>
        public byte VectorNumber;

        /// <summary>
        /// 指令+向量数最高位
        /// </summary>
        public byte Instruction;

        /// <summary>
        /// 操作数(包含操作数6byte+56byte向量)
        /// </summary>
        public byte[] Data;

        public PatternVectorGroupModel()
        {
            Data = new byte[62];
        }
    }
}
