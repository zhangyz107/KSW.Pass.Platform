using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Memory
{
    [Serializable]
    public class MemoryHeadInfo
    {
        /// <summary>
        /// 内存名称
        /// </summary>
        public string MemoryName { get; set; }

        /// <summary>
        /// 当前数据地址
        /// </summary>
        public long CurrentDataAddress { get; set; }

        /// <summary>
        /// 块集合
        /// </summary>
        public List<MemoryDataBlock> Blocks { get; set; } = new List<MemoryDataBlock>();
    }
}
