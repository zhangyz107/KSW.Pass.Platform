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
    /// 时钟组逻辑层接口
    /// </summary>
    public interface ITimingGroupBLL : IService
    {
        /// <summary>
        /// 通过Id获取时钟组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TimingGroupModel> GetByIdAsync(string id);

        /// <summary>
        /// 通过项目Id获取时钟组列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<List<TimingGroupModel>> GetListByProjectIdAsync(string projectId);

        /// <summary>
        /// 保存时钟组信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<TimingGroupModel> SaveAsync(TimingGroupModel model);

        /// <summary>
        /// 删除时钟组及子元
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteWithChildrenAsync(string id);
    }
}
