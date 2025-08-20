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
    /// 测试计划电平组逻辑层接口
    /// </summary>
    public interface ILevelGroupBLL : IService
    {
        /// <summary>
        /// 通过Id获取电平组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<LevelGroupModel> GetByIdAsync(string id);

        /// <summary>
        /// 通过项目Id获取电平组列表
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        Task<List<LevelGroupModel>> GetListByProjectIdAsync(string projectId);

        /// <summary>
        /// 保存电平组
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<LevelGroupModel> SaveAsync(LevelGroupModel model);

        /// <summary>
        /// 删除电平组及子元
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteWithChildrenAsync(string id);
    }
}
