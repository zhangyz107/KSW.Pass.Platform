using KSW.Application;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.ATE01.Project.Base.Models.TestPlans;

namespace KSW.ATE01.Application.BLLs.Abstractions.TestPlans
{
    /// <summary>
    /// 测试计划业务逻辑层接口
    /// </summary>
    public interface ITestPlanBLL : IService
    {
        /// <summary>
        /// 加载测试项
        /// </summary>
        Task<TestPlanModel> LoadTestPlanAsync(ProjectInfoModel projectInfo);

        /// <summary>
        /// 设置测试计划Flow
        /// </summary>
        bool SetTestPlanFlow(IList<FlowInfoModel> flows, ProjectInfoModel projectInfo);

        /// <summary>
        /// 另存为测试计划
        /// </summary>
        /// <param name="testPlan">测试计划</param>
        /// <param name="testPlanType">测试计划类型</param>
        /// <param name="saveAsDir">另存为文件夹</param>
        /// <param name="fileName">文件名</param>
        bool SaveAsTestPlan(TestPlanModel testPlan, TestPlanType testPlanType, string saveAsDir, string fileName);

    }
}
