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
    /// 引脚概览业务逻辑层接口
    /// </summary>
    public interface IPinOverviewBLL : IService
    {
        Task<PinOverviewModel> GetByIdAsync(string id);

        /// <summary>
        /// 从项目Id获取引脚概览
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PinOverviewModel> GetPinOverviewFromProjectIdAsync(string id);

        /// <summary>
        /// 保存引脚概览
        /// </summary>
        /// <param name="pinOverviewModel"></param>
        /// <returns></returns>
        Task<bool> SaveAsync(PinOverviewModel model);
    }
}
