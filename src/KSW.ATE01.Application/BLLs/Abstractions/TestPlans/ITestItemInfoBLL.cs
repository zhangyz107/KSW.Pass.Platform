using KSW.Application;
using KSW.ATE01.Application.Models.TestPlans;

namespace KSW.ATE01.Application.BLLs.Abstractions.TestPlans
{
    /// <summary>
    /// 测试项信息逻辑层接口
    /// </summary>
    public interface ITestItemInfoBLL : IService
    {
        /// <summary>
        /// 通过Id获取测试项信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TestItemInfoModel> GetByIdAsync(string id);

        /// <summary>
        /// 通过项目Id获取门限列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<List<TestItemInfoModel>> GetListByProjectIdAsync(string projectId);

        /// <summary>
        /// 创建测试项
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> CreateAsync(TestItemInfoModel model);

        /// <summary>
        /// 更新测试项
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<TestItemInfoModel> UpdateAsync(TestItemInfoModel model);

        /// <summary>
        /// 删除测试项信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);
    }
}
