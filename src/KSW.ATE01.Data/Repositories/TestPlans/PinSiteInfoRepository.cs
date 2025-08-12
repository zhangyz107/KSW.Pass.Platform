using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.Data.EntityFrameworkCore;

namespace KSW.ATE01.Data.Repositories.TestPlans
{
    public class PinSiteInfoRepository : RepositoryBase<PinSiteInfo>, IPinSiteInfoRepository
    {
        public PinSiteInfoRepository(ISystemUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
