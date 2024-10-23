using KSW.Application;
using KSW.ATE01.Application.Models.TestPlan;
using KSW.ATE01.Domain.Projects.Core.Enums;

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
        Task<TestPlanModel> LoadTestPlanAsync(TestPlanType testPlanType, string filePath);

        /// <summary>
        /// 设置测试计划Flow
        /// </summary>
        void SetTestPlanFlow(TestPlanModel testPlan, TestPlanType testPlanType, string filePath);
    }
}
