using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.Data.EntityFrameworkCore;

namespace KSW.ATE01.Data.Repositories.TestPlans
{
    public class PinOverviewRepository : RepositoryBase<PinOverview>, IPinOverviewRepository
    {
        public PinOverviewRepository(ISystemUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
