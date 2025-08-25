using KSW.Application;
using KSW.ATE01.Domain.TestPlan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Managers.Abstractions.TestPlans
{
    /// <summary>
    /// 门限管理接口
    /// </summary>
    public interface ILimitsManager : IService
    {
        /// <summary>
        /// 验证删除
        /// </summary>
        /// <param name="limits"></param>
        /// <returns></returns>
        Task ValidateDeleteAsync(List<Limits> limits);
    }
}
