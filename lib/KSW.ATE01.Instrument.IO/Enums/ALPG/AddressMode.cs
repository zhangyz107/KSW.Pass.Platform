using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.ALPG
{
    /// <summary>
    /// 地址模式
    /// </summary>
    public enum AddressMode
    {
        /// <summary>
        /// 直通
        /// </summary>
        [Description("直通")]
        PassThrough = 0,

        /// <summary>
        /// 查表
        /// </summary>
        [Description("查表")]
        LookUpTable = 1,

        /// <summary>
        /// 异或
        /// </summary>
        [Description("异或")]
        XOR = 2,
    }
}
