using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    /// <summary>
    /// 测试项总结模型
    /// </summary>
    public class TestItemSummaryModel
    {
        public string Site { get; set; }

        /// <summary>
        /// 测试项名称
        /// </summary>
        public string TestItemName { get; set; }

        /// <summary>
        /// 测试编号
        /// </summary>
        public string TestNumber { get; set; }

        /// <summary>
        /// 门限名称
        /// </summary>
        public string LimitName { get; set; }

        /// <summary>
        /// 引脚名称
        /// </summary>
        public string PinName { get; set; }

        /// <summary>
        /// 执行次数
        /// </summary>
        public int ExecutedCount { get; set; }

        /// <summary>
        /// 失败次数
        /// </summary>
        public int FailedCount { get; set; }

        /// <summary>
        /// 通过百分比
        /// </summary>
        public int PassedPercent { get; set; }
    }
}
