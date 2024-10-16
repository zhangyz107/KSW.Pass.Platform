/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：Flow.cs
// 功能描述：流程
//
// 作者：zhangyingzhong
// 日期：2024/10/16 10:37
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 流程
    /// </summary>
    public class Flow
    {
        /// <summary>
        /// 测试项Id
        /// </summary>
        public Guid TestItemId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int SortId { get; set; }

        /// <summary>
        /// 使能
        /// </summary>
        public bool Enable { get; set; }
    }
}
