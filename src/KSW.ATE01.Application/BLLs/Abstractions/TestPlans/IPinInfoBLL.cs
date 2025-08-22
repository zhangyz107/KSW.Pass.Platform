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
    /// 引脚信息业务逻辑层接口
    /// </summary>
    public interface IPinInfoBLL : IService
    {
        /// <summary>
        /// 通过引脚概览Id获取引脚信息列表
        /// </summary>
        /// <param name="overviewId">概览Id</param>
        /// <param name="ids">需要过滤掉的引脚Id</param>
        /// <returns></returns>
        Task<List<PinInfoModel>> GetPinInfosFromOvewviewIdAsync(string overviewId, List<string> ids = null);
    }
}
