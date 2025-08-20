using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
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
    /// 测试计划电平逻辑层
    /// </summary>
    public class LevelBLL : CrudServiceBase<Level>, ILevelBLL
    {
        private readonly ILevelRepository _repository;
        private readonly ILevelGroupRepository _levelGroupRepository;
        private readonly IGroupInfoRepository _groupInfoRepository;
        private readonly IPinInfoRepository _pinInfoRepository;

        public LevelBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            ILevelRepository repository,
            ILevelGroupRepository levelGroupRepository,
            IGroupInfoRepository groupInfoRepository,
            IPinInfoRepository pinInfoRepository) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
            _levelGroupRepository = levelGroupRepository;
            _groupInfoRepository = groupInfoRepository;
            _pinInfoRepository = pinInfoRepository;
        }

        public async Task<LevelModel> GetByIdAsync(string id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return entity?.MapTo<LevelModel>();
        }

        public async Task<List<LevelModel>> GetListByGroupIdAsync(string groupId)
        {
            var list = await _repository.FindAllAsync(x => x.LevelGroupId.Equals(groupId.ToGuid()));
            var levelGroupIds = list.Select(x => x.LevelGroupId).Distinct().ToList();
            var levelGroups = await _levelGroupRepository.FindAllAsync(x => levelGroupIds.Contains(x.Id));
            var ids = list.Select(x => x.GroupOrPinId).Distinct().ToList();
            var groups = await _groupInfoRepository.FindAllAsync(x => ids.Contains(x.Id));
            var pins = await _pinInfoRepository.FindAllAsync(x => ids.Contains(x.Id));
            var result = list?.MapToList<LevelModel>();

            foreach (var item in result)
            {
                item.LevelGroupName = levelGroups.FirstOrDefault(x => x.Id.Equals(item.LevelGroupId))?.LevelGroupName;
                item.PinOrGroupName = groups.FirstOrDefault(x => x.Id.Equals(item.GroupOrPinId))?.GroupName;
                if (item.PinOrGroupName.IsEmpty())
                    item.PinOrGroupName = pins.FirstOrDefault(x => x.Id.Equals(item.GroupOrPinId))?.PinName;
            }

            return result;
        }

        public async Task<string> CreateAsync(LevelModel model)
        {
            var entity = model.MapTo<Level>();
            await CreateAsync(entity);
            return entity.Id.SafeString();
        }

        public async Task<LevelModel> UpdateAsync(LevelModel model)
        {
            var entity = model.MapTo<Level>();
            await UpdateAsync(model.Id, entity);
            return await GetByIdAsync(model.Id);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.RemoveAsync(id);
            await CommitAsync();
            return;
        }
    }
}
