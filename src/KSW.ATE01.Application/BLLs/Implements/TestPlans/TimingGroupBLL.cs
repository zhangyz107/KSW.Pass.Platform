using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;

namespace KSW.ATE01.Application.BLLs.Implements.TestPlans
{
    /// <summary>
    /// 时钟组逻辑层
    /// </summary>
    public class TimingGroupBLL : CrudServiceBase<TimingGroup>, ITimingGroupBLL
    {
        private readonly ITimingGroupRepository _repository;
        private readonly ITimingRepository _timingRepository;
        private readonly ITimingGroupManager _timingGroupManager;

        public TimingGroupBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            ITimingGroupRepository repository,
            ITimingRepository timingRepository,
            ITimingGroupManager timingGroupManager) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
            _timingRepository = timingRepository;
            _timingGroupManager = timingGroupManager;
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
            var entities = await _repository.FindByIdsAsync(id);
            await DeleteBeforeAsync(entities);

            var timings = await _timingRepository.FindAllAsync(x => x.TimingGroupId.Equals(id.ToGuid()));
            if (!timings.IsEmpty())
                await _timingRepository.RemoveAsync(timings);

            await _repository.RemoveAsync(id);
            await CommitAsync();
        }

        private async Task DeleteBeforeAsync(List<TimingGroup> entities)
        {
            await _timingGroupManager?.ValidateDeleteAsync(entities);
        }
    }
}
