using KSW.ATE01.Domain.Projects.Core.Enums;

namespace KSW.ATE01.Domain.Projects.Entities
{
    [Serializable]
    public class ProjectInfo
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 测试计划类型
        /// </summary>
        public TestPlanType TestPlanType { get; set; }

        /// <summary>
        /// 项目路径
        /// </summary>
        public string ProjectPath { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 项目版本
        /// </summary>
        public string ProjectVersion { get; set; }

        /// <summary>
        /// 记录RealTime TxT
        /// </summary>
        public bool SaveRealTimeText { get; set; }

        /// <summary>
        /// 记录CSV
        /// </summary>
        public bool SaveCsv { get; set; }

        /// <summary>
        /// 记录Summary
        /// </summary>
        public bool SaveSummary { get; set; }

        /// <summary>
        /// 记录STDF
        /// </summary>
        public bool SaveSTDF { get; set; }

        /// <summary>
        /// 日志路径
        /// </summary>
        public string DatalogPath { get; set; }

        /// <summary>
        /// 是否DoAll
        /// </summary>
        public bool IsDoAll { get; set; }

        /// <summary>
        /// 是否打印时间
        /// </summary>
        public bool IsPrintTime { get; set; }

        /// <summary>
        /// 是否离线模式
        /// </summary>
        public bool IsOffLine { get; set; }

        /// <summary>
        /// 循环次数
        /// </summary>
        public int LoopCount { get; set; }

        /// <summary>
        /// 循环间时延
        /// </summary>
        public int DelayBetweenLoops { get; set; }

        /// <summary>
        /// 失败时停止
        /// </summary>
        public bool StopOnFail { get; set; }

        /// <summary>
        /// 发布路径
        /// </summary>
        public string ReleasePath { get; set; }
    }
}
