using KSW.Application;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.ATE01.Data.Repositories.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Managers.Implements.TestPlans
{
    public class GroupInfoManager : ServiceBase, IGroupInfoManager
    {
        private readonly ILevelRepository _levelRepository;
        private readonly ITimingRepository _timingRepository;
        private readonly ITestItemInfoRepository _testItemInfoRepository;

        public GroupInfoManager(
            IContainerProvider containerProvider,
            ILevelRepository levelRepository,
            ITimingRepository timingRepository,
            ITestItemInfoRepository testItemInfoRepository) : base(containerProvider)
        {
            _levelRepository = levelRepository;
            _timingRepository = timingRepository;
            _testItemInfoRepository = testItemInfoRepository;
        }

        public async Task ValidateDeleteAsync(List<GroupInfo> groupInfos)
        {
            foreach (var groupInfo in groupInfos)
            {
                var levels = await _levelRepository?.FindAllAsync(x => x.GroupOrPinId.Equals(groupInfo.Id));
                if (!levels.IsEmpty())
                {
                    var level = levels.FirstOrDefault();
                    throw new Exception(string.Format(L["IsOccupiedBy"], groupInfo.GroupName, L["Level"]));
                }

                var timings = await _timingRepository?.FindAllAsync(x => x.GroupOrPinId.Equals(groupInfo.Id));
                if (!timings.IsEmpty())
                {
                    var timing = timings.FirstOrDefault();
                    throw new Exception(string.Format(L["IsOccupiedBy"], groupInfo.GroupName, $"{L["TimingName"]}-{timing?.TimingName}"));
                }

                var testItems = await _testItemInfoRepository?.FindAllAsync(x => x.GroupOrPinId.Equals(groupInfo.Id));
                if (!testItems.IsEmpty())
                {
                    var testItem = testItems.FirstOrDefault();
                    throw new Exception(string.Format(L["IsOccupiedBy"], groupInfo.GroupName, $"{L["TestItemName"]}-{testItem?.TestItemName}"));
                }
            }
        }
    }
}
