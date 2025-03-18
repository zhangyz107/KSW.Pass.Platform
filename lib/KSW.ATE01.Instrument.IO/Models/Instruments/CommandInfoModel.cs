using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Instruments
{
    /// <summary>
    /// 指令信息模型
    /// </summary>
    public class CommandInfoModel
    {
        /// <summary>
        /// 指令Id
        /// </summary>
        public string CommandCode { get; set; }

        /// <summary>
        /// 槽位号
        /// </summary>
        public byte SlotNum { get; set; }

        /// <summary>
        /// 指令长度
        /// </summary>
        public short CommnadLength => (CommandContent != null && CommandContent.Any()) ? (short)CommandContent.Length : (short)0;

        /// <summary>
        /// 指令内容
        /// </summary>
        public byte[] CommandContent { get; set; }
    }
}
