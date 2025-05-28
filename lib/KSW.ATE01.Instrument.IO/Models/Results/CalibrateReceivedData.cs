using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    public class CalibrateReceivedData
    {
        /// <summary>
        /// 通道号
        /// </summary>
        public byte ChannelNum { get; set; }
        /// <summary>
        /// CA接收第一个数据
        /// </summary>
        public uint CA { get; set;}

        /// <summary>
        /// CB接收第一个数据
        /// </summary>
        public uint CB  { get; set;}

        /// <summary>
        /// CA接收1数量
        /// </summary>
        public uint CACount { get; set;}

        /// <summary>
        /// CB接收1数量
        /// </summary>
        public uint CBCount { get; set; }

        /// <summary>
        /// 第一个1出现位置
        /// </summary>
        public uint Position { get; set; }
    }
}
