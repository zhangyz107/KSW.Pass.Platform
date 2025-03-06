using KSW.ATE01.Instrument.IO.Enums.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Instruments
{
    /// <summary>
    /// 板卡信息模型
    /// </summary>
    public class BoardInfoModel
    {
        /// <summary>
        /// 板卡名
        /// </summary>
        public string BoardName { get; set; }

        /// <summary>
        /// 插槽号
        /// </summary>
        public string SlotNum { get; set; }

        /// <summary>
        /// 板卡类型
        /// </summary>
        public BoardType BoardType { get; set; }
    }
}
