using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.Data.Abstractions;
using KSW.Data.EntityFrameworkCore;

namespace KSW.ATE01.Data.Repositories.TestPlans
{
    public class PinGroupRelationshipRepositoy : RepositoryBase<PinGroupRelationship>, IPinGroupRelationshipRepositoy
    {
        public PinGroupRelationshipRepositoy(ISystemUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
