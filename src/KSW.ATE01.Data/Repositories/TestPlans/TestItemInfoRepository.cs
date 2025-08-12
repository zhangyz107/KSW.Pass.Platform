using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.Data.EntityFrameworkCore;

namespace KSW.ATE01.Data.Repositories.TestPlans
{
    public class TestItemInfoRepository : RepositoryBase<TestItemInfo>, ITestItemInfoRepository
    {
        public TestItemInfoRepository(ISystemUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
