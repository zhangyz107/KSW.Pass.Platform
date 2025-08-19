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
    /// 引脚站点业务逻辑接口
    /// </summary>
    public interface IPinSiteInfoBLL : IService
    {

        /// <summary>
        /// 通过引脚Id获取所有引脚站点
        /// </summary>
        /// <param name="overviewId">概览Id</param>
        /// <returns></returns>
        Task<List<PinSiteInfoModel>> GetAllPinSiteByOverviewIdAsync(string overviewId);

        /// <summary>
        /// 通过引脚Id获取引脚站点
        /// </summary>
        /// <param name="pinId"></param>
        /// <returns></returns>
        Task<List<PinSiteInfoModel>> GetPinSiteByPinIdAsync(string pinId);

        /// <summary>
        /// 保存引脚站点
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task SaveAsync(List<PinSiteInfoModel> createList, List<PinSiteInfoModel> updateList, List<PinSiteInfoModel> deleteList);
    }
}
