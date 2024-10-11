using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.Dtos;

namespace KSW.ATE01.Application.Models.Projects
{
    /// <summary>
    /// 项目信息模型
    /// </summary>
    public class ProjectInfoModel : DtoBase
    {
        private string _projectName;
        private TestPlanType _testPlanType;
        private string _projectPath;
        private DateTime _createTime;
        private string _projectVersion;

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName
        {
            get => _projectName;
            set => SetProperty(ref _projectName, value);
        }

        /// <summary>
        /// 测试计划类型
        /// </summary>
        public TestPlanType TestPlanType
        {
            get => _testPlanType;
            set => SetProperty(ref _testPlanType, value);
        }

        public string TestPlanTypeDescription => TestPlanType.Description();

        /// <summary>
        /// 项目路径
        /// </summary>
        public string ProjectPath
        {
            get => _projectPath;
            set => SetProperty(ref _projectPath, value);
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get => _createTime;
            set => SetProperty(ref _createTime, value);
        }

        /// <summary>
        /// 项目版本
        /// </summary>
        public string ProjectVersion
        {
            get => _projectVersion;
            set => SetProperty(ref _projectVersion, value);
        }

        /// <summary>
        /// 测试计划扩展名
        /// </summary>
        public string TestPlanExtension => _testPlanType == TestPlanType.Excel ? ".xlsx" : ".csv";

        /// <summary>
        /// 配置文件扩展名
        /// </summary>
        public string ConfigurationExtension => ".atecfg";

        /// <summary>
        /// 执行程序扩展名
        /// </summary>
        public string ExecuteExtension => ".dll";

    }
}
