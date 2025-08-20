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
    /// 测试计划电平逻辑层接口
    /// </summary>
    public interface ILevelBLL : IService
    {
        /// <summary>
        /// 通过Id获取电平信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<LevelModel> GetByIdAsync(string id);

        /// <summary>
        /// 通过分组Id获取电平列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<List<LevelModel>> GetListByGroupIdAsync(string groupId);

        /// <summary>
        /// 创建门限信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> CreateAsync(LevelModel model);

        /// <summary>
        /// 更新门限信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<LevelModel> UpdateAsync(LevelModel model);

        /// <summary>
        /// 删除门限信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);
    }
}
