using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    /// <summary>
    /// 计数结果模型
    /// </summary>
    public class CountValueModel
    {
        public int Index {  get; set; }

        public uint Value { get; set; }
    }
}
