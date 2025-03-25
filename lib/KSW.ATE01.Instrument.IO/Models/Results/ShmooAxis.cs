using KSW.ATE01.Instrument.IO.Enums.Shmoos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    public class ShmooAxis
    {
        /// <summary>
        /// 坐标轴类型
        /// </summary>
        public AxisType AxisType { get; set; }

        /// <summary>
        /// 模式
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// 轴起始值
        /// </summary>
        public double Begin { get; set; }

        /// <summary>
        /// 轴结束值
        /// </summary>
        public double End { get; set; }

        /// <summary>
        /// 步进值
        /// </summary>
        public double Step { get; set; }

        /// <summary>
        /// 引脚集
        /// </summary>
        public string PinList { get; set; }

        /// <summary>
        /// TimingSheet名
        /// </summary>
        public string TimingName { get; set; }

        /// <summary>
        /// 循环延时
        /// </summary>
        public double Delay { get; set; }

    }
}
