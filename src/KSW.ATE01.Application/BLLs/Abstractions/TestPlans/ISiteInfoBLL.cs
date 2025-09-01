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
    /// 站点信息业务逻辑层接口
    /// </summary>
    public interface ISiteInfoBLL : IService
    {
        /// <summary>
        /// 通过Id获取站点信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SiteInfoModel> GetByIdAsync(string id);

        /// <summary>
        /// 通过PinOverviewId获取站点信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<SiteInfoModel>> GetSiteInfosFromPinOverviewId(string id);

        /// <summary>
        /// 保存站点信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task CreateSiteByCountAsync(string pinOverviewId, int? count);

        /// <summary>
        /// 更新站点信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<SiteInfoModel> UpdateAsync(SiteInfoModel model);

        /// <summary>
        /// 批量保存站点信息
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        Task SaveAsync(List<SiteInfoModel> models);
    }
}
