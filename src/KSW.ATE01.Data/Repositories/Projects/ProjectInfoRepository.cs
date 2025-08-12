using KSW.ATE01.Domain.Projects.Entities;
using KSW.ATE01.Domain.Projects.Repositories;
using KSW.Data.EntityFrameworkCore;

namespace KSW.ATE01.Data.Repositories.Projects
{
    public class ProjectInfoRepository : RepositoryBase<ProjectInfo>, IProjectInfoRepository
    {
        public ProjectInfoRepository(ISystemUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
