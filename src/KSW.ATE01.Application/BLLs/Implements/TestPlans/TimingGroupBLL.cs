using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Data.Repositories.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.Data.Abstractions;
using KSW.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.BLLs.Implements.TestPlans
{
    /// <summary>
    /// 时钟组逻辑层
    /// </summary>
    public class TimingGroupBLL : CrudServiceBase<TimingGroup>, ITimingGroupBLL
    {
        private readonly ITimingGroupRepository _repository;
        private readonly ITimingRepository _timingRepository;

        public TimingGroupBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            ITimingGroupRepository repository,
            ITimingRepository timingRepository) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
            _timingRepository = timingRepository;
        }

        public async Task<TimingGroupModel> GetByIdAsync(string id)
        {
            var result = await _repository.FindByIdAsync(id);
            return result?.MapTo<TimingGroupModel>();
        }

        public async Task<List<TimingGroupModel>> GetListByProjectIdAsync(string projectId)
        {
            var liet = await _repository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            return liet?.MapToList<TimingGroupModel>();
        }

        public async Task<TimingGroupModel> SaveAsync(TimingGroupModel model)
        {
            var entity = model.MapTo<TimingGroup>();
            if (model.IsNew)
                await CreateAsync(entity);
            else
                await UpdateAsync(model.Id, entity);

            var id = model.Id ?? entity.Id.SafeString();
            return await GetByIdAsync(id);
        }

        public async Task DeleteWithChildrenAsync(string id)
        {
            var levels = await _timingRepository.FindAllAsync(x => x.TimingGroupId.Equals(id.ToGuid()));
            if (!levels.IsEmpty())
                await _timingRepository.RemoveAsync(levels);

            await _repository.RemoveAsync(id);
            await CommitAsync();
        }
    }
}
