/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：Limits.cs
// 功能描述：电压限制
//
// 作者：zhangyingzhong
// 日期：2024/10/16 10:28
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Domain.TestPlan.Core.Enums;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 电压限制
    /// </summary>
    public class Limits
    {
        /// <summary>
        /// 测试项Id
        /// </summary>
        public Guid TestItemId { get; set; }

        /// <summary>
        /// 电压限制名称
        /// </summary>
        public string LimitName { get; set; }

        /// <summary>
        /// 测试编号
        /// </summary>
        public int TestNumber { get; set; }

        /// <summary>
        /// 电压下限
        /// </summary>
        public decimal LowLimit { get; set; }

        /// <summary>
        /// 电压上限
        /// </summary>
        public decimal HighLimit { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 软件Bin号
        /// </summary>
        public int FailSoftwareBin { get; set; }

        /// <summary>
        /// 软件Bin号
        /// </summary>
        public int PassSoftwareBin { get; set; }

        /// <summary>
        /// 硬件Bin号
        /// </summary>
        public int FailHardwareBin { get; set; }

        /// <summary>
        /// 硬件Bin号
        /// </summary>
        public int PassHardwareBin { get; set; }

        /// <summary>
        /// 测试结果
        /// </summary>
        public DUTResultType DUTResult { get; set; }
    }
}
