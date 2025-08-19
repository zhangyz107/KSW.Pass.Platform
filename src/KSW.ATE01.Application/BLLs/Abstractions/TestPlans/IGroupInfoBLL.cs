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
    /// 组信息业务逻辑接口
    /// </summary>
    public interface IGroupInfoBLL : IService
    {
        /// <summary>
        /// 通过Id获取组信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<GroupInfoModel> GetByIdAsync(string id);

        /// <summary>
        /// 通过概览Id获取组信息列表
        /// </summary>
        /// <param name="overviewId"></param>
        /// <returns></returns>
        Task<List<GroupInfoModel>> GetListByOverviewIdAsync(string overviewId);

        /// <summary>
        /// 保存组信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> CreateAsync(GroupInfoModel model);

        /// <summary>
        /// 更新组信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<GroupInfoModel> UpdateAsync(GroupInfoModel model);

        /// <summary>
        /// 删除组信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task DeleteWithDetailAsync(string id);

    }
}
