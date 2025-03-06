using System;

namespace KSW.ATE01.Project.Base.Models.TestPlans
{
    /// <summary>
    /// 流程模型
    /// </summary>
    [Serializable]
    public class FlowModel
    {
        /// <summary>
        /// 标识符
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 测试项Id
        /// </summary>
        public Guid TestItemId { get; set; }

        /// <summary>
        /// 测试项名称
        /// </summary>
        public string TestItemName { get; set; }

        /// <summary>
        /// Sheet名称
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int SortId { get; set; }

        /// <summary>
        /// 使能
        /// </summary>
        public string Enable { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected { get; set; }

    }
}
