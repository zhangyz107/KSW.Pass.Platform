using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    /// <summary>
    /// Pattern结果模型
    /// </summary>
    public class PatternResultModel
    {
        /// <summary>
        /// 存放起始地址
        /// </summary>
        public ushort StartAddress { get; set; }

        /// <summary>
        /// Pattern数据长度
        /// </summary>
        public short Length { get; set; }

        /// <summary>
        /// Pattern数据
        /// </summary>
        public byte[] Data { get; set; }
    }
}
