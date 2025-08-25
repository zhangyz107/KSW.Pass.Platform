using KSW.Application;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;

namespace KSW.ATE01.Application.Managers.Abstractions.TestPlans
{
    /// <summary>
    /// 引脚通道管理接口
    /// </summary>
    public interface IPinChannelManager : IService
    {
        /// <summary>
        /// 创建引脚和引脚的站点信息
        /// </summary>
        /// <param name="pinInfo"></param>
        /// <param name="pinSiteInfos"></param>
        /// <returns></returns>
        Task<string> CreatePinAndSiteInfoAsync(PinInfoModel pinInfo);

        /// <summary>
        /// 更新引脚和引脚的站点信息
        /// </summary>
        /// <param name="pinInfo"></param>
        /// <param name="pinSiteInfos"></param>
        /// <returns></returns>
        Task UpdatePinAndSiteInfoAsync(PinInfoModel pinInfo);

        /// <summary>
        /// 通过引脚Id删除引脚和引脚的站点信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeletePinAndSiteInfoByIdAsync(string pinId);
    }
}
