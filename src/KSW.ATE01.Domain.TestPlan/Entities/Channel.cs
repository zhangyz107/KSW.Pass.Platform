/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：Channel.cs
// 功能描述：通道设置
//
// 作者：zhangyingzhong
// 日期：2024/10/16 10:27
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Domain.TestPlan.Core.Enums;
using System.ComponentModel;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 通道设置
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// 组Id
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 引脚Id
        /// </summary>
        public Guid PinId { get; set; }

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
        [DisplayName("Site")]
        public List<Site> Sites { get; set; } = new List<Site>();
    }
}
