using KSW.Application;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Managers.Implements.TestPlans
{
    /// <summary>
    /// 门限管理
    /// </summary>
    public class LimitsManager : ServiceBase, ILimitsManager
    {
        private readonly ITestItemInfoRepository _testItemInfoRepository;

        public LimitsManager(
            IContainerProvider containerProvider,
            ITestItemInfoRepository testItemInfoRepository) : base(containerProvider)
        {
            _testItemInfoRepository = testItemInfoRepository;
        }

        public async Task ValidateDeleteAsync(List<Limits> limits)
        {
            foreach (var limit in limits)
            {
                var testItems = await _testItemInfoRepository?.FindAllAsync(x => x.LimitsId.Equals(limit.Id));
                if (!testItems.IsEmpty())
                {
                    var testItem = testItems?.FirstOrDefault();
                    throw new Exception(string.Format(L["IsOccupiedBy"], limit.LimitName, $"{L["TestItemName"]}-{testItem?.TestItemName}" ));
                }
            }
        }
    }
}
