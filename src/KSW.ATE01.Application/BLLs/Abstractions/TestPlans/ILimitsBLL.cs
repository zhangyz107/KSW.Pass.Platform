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
    /// 门限业务逻辑层接口
    /// </summary>
    public interface ILimitsBLL : IService
    {
        /// <summary>
        /// 通过Id获取门限信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<LimitsModel> GetByIdAsync(string id);

        /// <summary>
        /// 通过项目Id获取门限列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<List<LimitsModel>> GetListByProjectIdAsync(string projectId);

        /// <summary>
        /// 创建门限信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> CreateAsync(LimitsModel model);

        /// <summary>
        /// 更新门限信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<LimitsModel> UpdateAsync(LimitsModel model);

        /// <summary>
        /// 删除门限信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);

    }
}
