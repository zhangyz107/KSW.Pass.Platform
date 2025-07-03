using KSW.ATE01.Instrument.IO.Enums.Dps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    /// <summary>
    /// 获取Dps MI结果
    /// </summary>
    public class DpsMIResultModel<T> : ChannelResultModel<T>
    {
        public IRType IR { get; set; }
    }
}
