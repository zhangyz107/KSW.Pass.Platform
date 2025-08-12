using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.Data.Abstractions;
using KSW.Data.EntityFrameworkCore;

namespace KSW.ATE01.Data.Repositories.TestPlans
{
    public class LimitsRepository : RepositoryBase<Limits>, ILimitsRepository
    {
        public LimitsRepository(ISystemUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
