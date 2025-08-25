using KSW.Application;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;

namespace KSW.ATE01.Application.Managers.Implements.TestPlans
{
    public class LevelGroupManager : ServiceBase, ILevelGroupManager
    {
        private readonly ITestItemInfoRepository _testItemInfoRepository;

        public LevelGroupManager(
            IContainerProvider containerProvider,
            ITestItemInfoRepository testItemInfoRepository) : base(containerProvider)
        {
            _testItemInfoRepository = testItemInfoRepository;
        }

        public async Task ValidateDeleteAsync(List<LevelGroup> levelGroups)
        {
            foreach (var levelGroup in levelGroups)
            {
                var testItems = await _testItemInfoRepository?.FindAllAsync(x => x.LevelGroupId.Equals(levelGroup.Id));
                if (!testItems.IsEmpty())
                {
                    var testItem = testItems?.FirstOrDefault();
                    throw new Exception(string.Format(L["IsOccupiedBy"], levelGroup.LevelGroupName, $"{L["TestItemName"]}-{testItem?.TestItemName}" ));
                }
            }
        }
    }
}
