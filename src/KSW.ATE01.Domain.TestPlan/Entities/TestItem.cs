/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：TestItem.cs
// 功能描述：测试项
//
// 作者：zhangyingzhong
// 日期：2024/10/16 10:28
// 修改记录(Revision History)
//
//------------------------------------------------------------*/


using System.ComponentModel;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 测试项
    /// </summary>
    public class TestItem
    {
        /// <summary>
        /// 测试项Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 测试项名称
        /// </summary>
        public string TestItemName { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// 强制值
        /// </summary>
        public decimal Force { get; set; }

        /// <summary>
        /// 待测引脚
        /// </summary>
        public string Pins { get; set; }

        /// <summary>
        /// 电压参数集合
        /// </summary>
        [DisplayName("Level")]
        public List<Level> Levels { get; set; } = new List<Level>();

        /// <summary>
        /// 时钟参数集合
        /// </summary>
        [DisplayName("Timing")]
        public List<Timing> Timings { get; set; } = new List<Timing>();

        /// <summary>
        /// 参数
        /// </summary>
        public List<string> Args { get; set; } = new List<string>();
    }
}
