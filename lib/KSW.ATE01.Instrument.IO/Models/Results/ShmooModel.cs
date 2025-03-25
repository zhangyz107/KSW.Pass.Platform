using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    [Serializable]
    public class ShmooModel
    {
        /// <summary>
        /// Shmoo图信息
        /// </summary>
        public ShmooTest TestInfo { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public List<ShmooResult> Results { get; set; }
    }
}
