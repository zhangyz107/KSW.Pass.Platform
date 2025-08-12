using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.Data.Abstractions;
using KSW.Data.EntityFrameworkCore;

namespace KSW.ATE01.Data.Repositories.TestPlans
{
    public class LevelGroupRepository : RepositoryBase<LevelGroup>, ILevelGroupRepository
    {
        public LevelGroupRepository(ISystemUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
