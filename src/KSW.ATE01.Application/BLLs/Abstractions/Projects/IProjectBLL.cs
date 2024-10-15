using KSW.Application;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Domain.Projects.Core.Enums;

namespace KSW.ATE01.Application.BLLs.Abstractions
{
    /// <summary>
    /// 项目业务逻辑层接口
    /// </summary>
    public interface IProjectBLL : IService
    {
        /// <summary>
        /// 创建项目
        /// </summary>
        Task<bool> CreateProjectAsync(ProjectInfoModel projectInfo);

        /// <summary>
        /// 通过VS运行当前项目
        /// </summary>
        void RunProjecctByVS();

        /// <summary>
        /// 获取当前项目
        /// </summary>
        ProjectInfoModel GetCurrentProjectInfo();

        /// <summary>
        /// 扫描目标文件夹下的所有项目
        /// </summary>
        List<ProjectInfoModel> ScanProjects(string folderName);

        /// <summary>
        /// 保存项目信息
        /// </summary>
        bool SaveProjectInfo(ProjectInfoModel projectInfo);

        /// <summary>
        /// 从项目配置文件中加载项目信息
        /// </summary>
        ProjectInfoModel LoadProjectInfo(string file);

        /// <summary>
        /// 设置当前项目信息
        /// </summary>
        /// <param name="projectInfo"></param>
        void SetCurrentProjectInfo(ProjectInfoModel projectInfo);

        /// <summary>
        /// 另存为项目信息
        /// </summary>
        Task<bool> SaveAsProjectInfoAsync(TestPlanType testPlanType, string saveAsDir, string saveAsName);
    }
}
