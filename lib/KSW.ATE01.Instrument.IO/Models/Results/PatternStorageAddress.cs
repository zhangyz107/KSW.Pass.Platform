using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    /// <summary>
    /// 向量结果数据存储地址
    /// </summary>
    public class PatternStorageAddress
    {
        /// <summary>
        /// 起始地址
        /// </summary>
        public long StartAddress { get; set; }

        /// <summary>
        /// 截止地址
        /// </summary>
        public long EndAddress { get; set; }
    }
}
