using KSW.Application;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.Interception;

namespace KSW.ATE01.Application.BLLs.Abstractions.Projects
{
    /// <summary>
    /// 项目业务逻辑层接口
    /// </summary>
    public interface IProjectBLL : IService
    {
        /// <summary>
        /// 通过id获取项目信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ProjectInfoModel> GetByIdAsync(string id);

        /// <summary>
        /// 获取项目列表
        /// </summary>
        /// <returns></returns>
        Task<List<ProjectInfoModel>> GetListAsync();

        /// <summary>
        /// 创建项目
        /// </summary>
        Task<string> CreateAsync(ProjectInfoModel projectInfo);

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
        //List<ProjectInfoModel> ScanProjects(string folderName);

        /// <summary>
        /// 保存项目信息
        /// </summary>
        Task<ProjectInfoModel> UpdateAsync(ProjectInfoModel projectInfo);

        /// <summary>
        /// 从项目配置文件中加载项目信息
        /// </summary>
        //ProjectInfoModel LoadProjectInfo(string file);

        /// <summary>
        /// 设置当前项目信息
        /// </summary>
        /// <param name="projectInfo"></param>
        void SetCurrentProjectInfo(ProjectInfoModel projectInfo);

        /// <summary>
        /// 发布解决方案
        /// </summary>
        Task<bool> ReleaseSolutionAsync(ProjectInfoModel projectInfo = null, bool openReleaseDir = false);

        /// <summary>
        /// 拷贝测试计划
        /// </summary>
        Task<bool> CopyTestPlanAsync(ProjectInfoModel projectInfo = null);

        /// <summary>
        /// 开始测试
        /// </summary>
        Task StartTestAsync(List<FlowInfoModel> flows, ProjectInfoModel projectInfo = null);

        /// <summary>
        /// 结束测试
        /// </summary>
        Task EndTestAsync(ProjectInfoModel projectInfo = null);

        /// <summary>
        /// 循环执行
        /// </summary>
        Task ExecuteLoopingAsync(List<FlowInfoModel> flows, ProjectInfoModel projectInfo = null);

        /// <summary>
        /// 停止循环
        /// </summary>
        void StopLooping();

        /// <summary>
        /// 通过id删除项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);
    }
}
