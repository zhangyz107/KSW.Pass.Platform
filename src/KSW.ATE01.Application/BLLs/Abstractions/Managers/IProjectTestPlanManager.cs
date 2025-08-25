using KSW.Application;
using KSW.ATE01.Domain.Projects.Core.Enums;

namespace KSW.ATE01.Application.BLLs.Abstractions.Managers
{
    /// <summary>
    /// 项目测试计划管理接口
    /// </summary>
    public interface IProjectTestPlanManager : IService
    {
        /// <summary>
        /// 另存为项目信息
        /// </summary>
        Task<bool> SaveAsProjectInfoAsync(TestPlanType testPlanType, string saveAsDir, string saveAsName);
    }
}
