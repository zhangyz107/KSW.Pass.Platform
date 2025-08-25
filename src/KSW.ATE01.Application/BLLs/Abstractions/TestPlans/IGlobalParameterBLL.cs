using KSW.Application;
using KSW.ATE01.Application.Models.TestPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.BLLs.Abstractions.TestPlans
{
    /// <summary>
    /// 全局参数逻辑层接口
    /// </summary>
    public interface IGlobalParameterBLL : IService
    {
        /// <summary>
        /// 通过Id获取全局参数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<GlobalParameterModel> GetByIdAsync(string id);

        /// <summary>
        /// 通过项目Id获取门限列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<List<GlobalParameterModel>> GetListByProjectIdAsync(string projectId);

        /// <summary>
        /// 创建全局参数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> CreateAsync(GlobalParameterModel model);

        /// <summary>
        /// 更新全局参数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<GlobalParameterModel> UpdateAsync(GlobalParameterModel model);

        /// <summary>
        /// 删除测试项信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);
    }
}
