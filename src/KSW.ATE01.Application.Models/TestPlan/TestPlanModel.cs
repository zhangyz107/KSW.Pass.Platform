using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlan
{
    /// <summary>
    /// 测试计划模型
    /// </summary>
    public class TestPlanModel
    {
        /// <summary>
        /// 通道
        /// </summary>
        public List<ChannelModel> Channel { get; set; }

        /// <summary>
        /// 测试项
        /// </summary>
        public List<TestItemModel> TestItem { get; set; }

        /// <summary>
        /// 电压阈值
        /// </summary>
        public List<LimitsModel> Limits { get; set; }

        /// <summary>
        /// 流程
        /// </summary>
        public List<FlowModel> Flow { get; set; }
    }
}
