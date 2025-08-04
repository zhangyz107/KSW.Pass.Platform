using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.DataLogs
{
    /// <summary>
    /// 数据记录枚举类型
    /// </summary>
    public enum DataLogType
    {
        /// <summary>
        /// Text格式
        /// </summary>
        Txt,

        /// <summary>
        /// CSV格式
        /// </summary>
        Csv,

        /// <summary>
        /// STDF格式
        /// </summary>
        STDF,

        /// <summary>
        /// Summary格式
        /// </summary>
        Summary,
    }
}
