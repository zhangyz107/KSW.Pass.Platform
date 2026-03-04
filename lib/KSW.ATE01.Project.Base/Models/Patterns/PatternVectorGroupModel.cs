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
    [Serializable]
    public class PatternVectorGroupModel
    {
        /// <summary>
        /// 向量数
        /// </summary>
        public byte VectorNumber { get; set; }

        /// <summary>
        /// 指令+向量数最高位
        /// </summary>
        public byte Instruction { get; set; }

        /// <summary>
        /// 操作数(包含操作数6byte+56byte向量)
        /// </summary>
        public byte[] Data { get; set; } = new byte[62];
    }
}
