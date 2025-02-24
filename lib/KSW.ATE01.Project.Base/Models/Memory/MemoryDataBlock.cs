using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Memory
{
    [Serializable]
    public class MemoryDataBlock
    {
        /// <summary>
        /// 起始地址
        /// </summary>
        public long StartAddress { get; set; }

        /// <summary>
        /// 块长度
        /// </summary>
        public long BlockLength { get; set; }

        /// <summary>
        /// 块名称
        /// </summary>
        public string BlockName { get; set; }

        /// <summary>
        /// 有效值长度
        /// </summary>
        public int VaildValueLength { get; set; }

        /// <summary>
        /// 块内容
        /// </summary>
        public byte[] BlockValue { get; set; }

        /// <summary>
        /// 块内容长度
        /// </summary>
        public int BlockValueLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BlockHoldValueSize { get; set; }
    }
}
