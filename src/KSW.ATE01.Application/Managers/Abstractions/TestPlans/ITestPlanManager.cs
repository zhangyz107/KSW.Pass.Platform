using KSW.Application;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Project.Base.Models.TestPlans;

namespace KSW.ATE01.Application.Managers.Abstractions.TestPlans
{
    /// <summary>
    /// 测试计划业务逻辑层接口
    /// </summary>
    public interface ITestPlanManager : IService
    {
        /// <summary>
        /// 加载测试项
        /// </summary>
        Task<TestPlanModel> LoadTestPlanAsync(ProjectInfoModel projectInfo);

        /// <summary>
        /// 通过项目Id拷贝测试计划
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="newProjectId"></param>
        /// <returns></returns>
        Task CopyTestPlanByProjectIdAsync(string projectId, string newProjectId);

        /// <summary>
        /// 通过项目Id删除测试计划(不考虑任何级联关系)
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task DeleteTestPlanByProjectIdAsync(string projectId);
    }
}
