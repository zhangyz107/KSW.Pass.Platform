using KSW.ATE01.Project.Base.Enums.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Patterns
{
    /// <summary>
    /// Pattern组模型
    /// </summary>
    public class PatternGroupModel
    {
        /// <summary>
        /// Vector数量
        /// </summary>
        public int VectorNumber { get => Vectors.Any() ? Vectors.Count * 2 : 0; }

        /// <summary>
        /// 指令
        /// </summary>
        public CommandType Instruction { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public List<byte> Parameter { get; set; } = new List<byte>();

        /// <summary>
        /// 向量值
        /// </summary>
        public List<byte> Vectors { get; set; } = new List<byte>();
    }
}
