using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    /// <summary>
    /// 向量运行状态模型
    /// </summary>
    public class PatternRunningStateModel : ChannelResultModel<bool>
    {
        /// <summary>
        /// 是否执行状态
        /// </summary>
        public bool IsRunning { get; set; }
    }
}
