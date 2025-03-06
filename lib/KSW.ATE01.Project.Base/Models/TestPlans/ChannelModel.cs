using KSW.ATE01.Project.Base.Enums.TestPlans;
using System;
using System.Collections.Generic;

namespace KSW.ATE01.Project.Base.Models.TestPlans
{
    /// <summary>
    /// 通道模型
    /// </summary>
    [Serializable]
    public class ChannelModel
    {
        /// <summary>
        /// 标识符
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 组
        /// </summary>
        public List<PinGroupModel> Groups { get; set; } = new List<PinGroupModel>();

        /// <summary>
        /// 引脚名称
        /// </summary>
        public string PinName { get; set; }

        /// <summary>
        /// 通道类型
        /// </summary>
        public ChannelType Type { get; set; }

        /// <summary>
        /// Sites信息
        /// </summary>
        public List<SiteModel> Sites { get; set; } = new List<SiteModel>();
    }
}
