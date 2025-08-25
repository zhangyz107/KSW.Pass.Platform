using KSW.Application;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Managers.Implements.TestPlans
{
    public class TimingGroupManager : ServiceBase, ITimingGroupManager
    {
        private readonly ITestItemInfoRepository _testItemInfoRepository;

        public TimingGroupManager(
            IContainerProvider containerProvider,
            ITestItemInfoRepository testItemInfoRepository) : base(containerProvider)
        {
            _testItemInfoRepository = testItemInfoRepository;
        }

        public async Task ValidateDeleteAsync(List<TimingGroup> timingGroups)
        {
            foreach (var timingGroup in timingGroups)
            {
                var testItems = await _testItemInfoRepository?.FindAllAsync(x => x.TimingGroupId.Equals(timingGroup.Id));
                if (!testItems.IsEmpty())
                {
                    var testItem = testItems?.FirstOrDefault();
                    throw new Exception(string.Format(L["IsOccupiedBy"], timingGroup.TimingGroupName, $"{L["TestItemName"]}-{testItem?.TestItemName}"));
                }
            }
        }
    }
}
