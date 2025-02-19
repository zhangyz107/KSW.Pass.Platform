using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    public abstract class ResultBaseModel
    {
        /// <summary>
        /// 通道号
        /// </summary>
        public int ChannelNum { get; set; }

        /// <summary>
        /// 原始数据
        /// </summary>
        public byte[] OriginalData { get; set; }
    }
}
