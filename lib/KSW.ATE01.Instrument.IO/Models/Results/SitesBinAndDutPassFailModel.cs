using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    public class SitesBinAndDutPassFailModel
    {
        /// <summary>
        /// 站点名称
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// 站点
        /// </summary>
        public string Site { get; set; }

        public string HardwareBin { get; set; }

        public string SoftwareBin { get; set; }

        /// <summary>
        /// 测试项个数
        /// </summary>
        public int TestedItemCount { get; set; }

        /// <summary>
        /// Bin设置标志
        /// </summary>
        public string BinSetFlag { get; set; }

        /// <summary>
        /// 测试结果
        /// </summary>
        public string DutPassFail { get; set; }

        /// <summary>
        /// 探针X
        /// </summary>
        public string ProberX { get; set; }

        /// <summary>
        /// 探针Y
        /// </summary>
        public string ProberY { get; set; }

        /// <summary>
        /// 探针Z
        /// </summary>
        public string ProberZ { get; set; }
    }
}
