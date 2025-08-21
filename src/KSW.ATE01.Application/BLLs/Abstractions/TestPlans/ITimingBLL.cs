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
    /// 时钟逻辑层接口
    /// </summary>
    public interface ITimingBLL : IService
    {
        /// <summary>
        /// 通过Id获取时钟信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TimingModel> GetByIdAsync(string id);

        /// <summary>
        /// 通过分组Id获取时钟列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<List<TimingModel>> GetListByGroupIdAsync(string groupId);

        /// <summary>
        /// 创建时钟信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> CreateAsync(TimingModel model);

        /// <summary>
        /// 更新时钟信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<TimingModel> UpdateAsync(TimingModel model);

        /// <summary>
        /// 删除时钟信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);
    }
}
