/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：Timing.cs
// 功能描述：时钟
//
// 作者：zhangyingzhong
// 日期：2024/10/16 10:35
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Domain.TestPlan.Core.Enums;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 时钟
    /// </summary>
    public class Timing
    {
        /// <summary>
        /// 时钟名称
        /// </summary>
        public string TimingName { get; set; }

        /// <summary>
        /// 周期
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// 引脚Id
        /// </summary>
        public Guid PinId { get; set; }

        /// <summary>
        /// 限制名称
        /// </summary>
        public string PinSetup { get; set; }

        /// <summary>
        /// 波形格式
        /// </summary>
        public string Fmt { get; set; }

        /// <summary>
        /// DriveA
        /// </summary>
        public string DriveA { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DriveB { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DriveC { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DriveD { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public StrobeModeType StrobeMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int StrobeA { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int StrobeB { get; set; }
    }
}
