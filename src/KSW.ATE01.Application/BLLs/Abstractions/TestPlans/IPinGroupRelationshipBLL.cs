using KSW.Application;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.BLLs.Abstractions.TestPlans
{
    /// <summary>
    /// 引脚组关系业务逻辑层接口
    /// </summary>
    public interface IPinGroupRelationshipBLL : IService
    {
        /// <summary>
        /// 通过Id获取组信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PinGroupRelationshipModel> GetByIdAsync(string id);

        /// <summary>
        /// 通过组Id获取引脚和组关系信息集合
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<List<PinGroupRelationshipModel>> GetListByGroupIdAsync(string groupId);

        /// <summary>
        /// 通过组Id获取引脚和组关系信息集合
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<List<PinGroupRelationshipModel>> GetListByPinIdAsync(string pinId);

        /// <summary>
        /// 保存组和引脚关系
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> SaveAsync(PinGroupRelationshipModel model);

        /// <summary>
        /// 删除组和引脚关系
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);
    }
}
