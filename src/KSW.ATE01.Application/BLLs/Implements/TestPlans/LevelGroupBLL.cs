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
    /// 测试计划电平组逻辑层
    /// </summary>
    public class LevelGroupBLL : CrudServiceBase<LevelGroup>, ILevelGroupBLL
    {
        private readonly ILevelGroupRepository _repository;
        private readonly ILevelRepository _levelRepository;
        private readonly ILevelGroupManager _levelGroupManager;

        public LevelGroupBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            ILevelGroupRepository repository,
            ILevelRepository levelRepository,
            ILevelGroupManager levelGroupManager) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
            _levelRepository = levelRepository;
            _levelGroupManager = levelGroupManager;
        }

        public async Task<LevelGroupModel> GetByIdAsync(string id)
        {
            var result = await _repository.FindByIdAsync(id);
            return result?.MapTo<LevelGroupModel>();
        }

        public async Task<List<LevelGroupModel>> GetListByProjectIdAsync(string projectId)
        {
            var liet = await _repository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            return liet?.MapToList<LevelGroupModel>();
        }

        public async Task<LevelGroupModel> SaveAsync(LevelGroupModel model)
        {
            var entity = model.MapTo<LevelGroup>();
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

            var levels = await _levelRepository.FindAllAsync(x => x.LevelGroupId.Equals(id.ToGuid()));
            if (!levels.IsEmpty())
                await _levelRepository.RemoveAsync(levels);

            await _repository.RemoveAsync(id);
            await CommitAsync();
        }

        #region 删除前事件
        private async Task DeleteBeforeAsync(List<LevelGroup> entities)
        {
            await _levelGroupManager?.ValidateDeleteAsync(entities);
        }
        #endregion

    }
}
