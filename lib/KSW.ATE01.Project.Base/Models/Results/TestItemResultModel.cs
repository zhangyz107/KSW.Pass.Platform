using KSW.ATE01.Project.Base.Models.TestPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Results
{
    /// <summary>
    /// 测试项结果
    /// </summary>
    public class TestItemResultModel : LimitsModel
    {
        /// <summary>
        /// 测试值
        /// </summary>
        public double TestValue { get; set; }

        /// <summary>
        /// 引脚名称
        /// </summary>
        public string PinName { get; set; }

        /// <summary>
        /// 通道名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 日志
        /// </summary>
        public string Log { get; set; }

        /// <summary>
        /// X坐标
        /// </summary>
        public string XCoordinate { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public string YCoordinate { get; set; }
    }
}
