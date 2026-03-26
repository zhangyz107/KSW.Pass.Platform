using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Patterns
{
    /// <summary>
    /// Pattern包结构
    /// </summary>
    public struct PatternPackageStruct
    {
        /// <summary>
        /// 引脚名
        /// </summary>
        public string PinName;

        /// <summary>
        /// 起始地址
        /// </summary>
        public byte[] Address;

        /// <summary>
        /// 数据长度
        /// </summary>
        public ushort Length;

        /// <summary>
        /// 数据长度（字节表示）
        /// </summary>
        public byte[] LengthBytes;

        /// <summary>
        /// 数据
        /// </summary>
        public List<PatternVectorGroupModel> PatternGroups;

        public PatternPackageStruct()
        {
            PatternGroups = new List<PatternVectorGroupModel>();
        }
    }
}
