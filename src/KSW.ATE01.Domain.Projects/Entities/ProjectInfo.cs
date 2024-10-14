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
    }
}
