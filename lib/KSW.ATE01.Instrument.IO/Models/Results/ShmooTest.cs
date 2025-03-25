using KSW.ATE01.Instrument.IO.Enums.Shmoos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    [Serializable]
    public class ShmooTest
    {
        /// <summary>
        /// 测试名
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// X轴
        /// </summary>
        public ShmooAxis XAxis { get; set; }

        /// <summary>
        /// Y轴
        /// </summary>
        public ShmooAxis YAxis { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        public AxisDirection Direction { get; set; }

        /// <summary>
        /// 打印路径
        /// </summary>
        public string PrintPath { get; set; }
    }
}
