using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.Data.EntityFrameworkCore;

namespace KSW.ATE01.Data.Repositories.TestPlans
{
    public class TimingRepository : RepositoryBase<Timing>, ITimingRepository
    {
        public TimingRepository(ISystemUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
