/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：Level.cs
// 功能描述：电压
//
// 作者：zhangyingzhong
// 日期：2024/10/16 10:38
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 电压
    /// </summary>
    public class Level
    {
        /// <summary>
        /// 组Id
        /// </summary>
        public Guid ChannelGroupId { get; set; }

        /// <summary>
        /// 输入低电压
        /// </summary>
        public decimal Vil { get; set; }

        /// <summary>
        /// 输入高电压
        /// </summary>
        public decimal Vih { get; set; }

        /// <summary>
        /// 输出低电压
        /// </summary>
        public decimal Vol { get; set; }

        /// <summary>
        /// 输出高电压
        /// </summary>
        public decimal Voh { get; set; }

        /// <summary>
        /// 输出低电流
        /// </summary>
        public decimal Iol { get; set; }

        /// <summary>
        /// 输出高电流
        /// </summary>
        public decimal Ioh { get; set; }

        /// <summary>
        /// 电压基准
        /// </summary>
        public decimal Vt { get; set; }

        /// <summary>
        /// 钳位低电压
        /// </summary>
        public decimal Vcl { get; set; }

        /// <summary>
        /// 钳位高电压
        /// </summary>
        public decimal Vch { get; set; }


    }
}
