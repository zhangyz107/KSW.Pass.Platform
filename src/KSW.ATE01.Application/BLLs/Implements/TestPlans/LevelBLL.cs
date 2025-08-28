/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：LevelBLL.cs
// 功能描述：测试计划电平逻辑层
//
// 作者：zhangyingzhong
// 日期：2025/08/20 13:41
// 修改记录(Revision History)
//
//------------------------------------------------------------*/


using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.ATE01.Project.Base.Models.Errors;

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
            var list = (await _repository.FindAllAsync(x => x.LevelGroupId.Equals(groupId.ToGuid())))?.OrderBy(x=>x.CreationTime);
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
        }

        #region 创建前事件
        protected override async Task CreateBeforeAsync(Level entity)
        {
            var message = string.Empty;
            if (entity.Vil > entity.Vih)
                message = string.Format(L["ExceedValueError"], nameof(LevelModel.Vil), nameof(LevelModel.Vih));

            if (entity.Vol > entity.Voh)
                message = string.Format(L["ExceedValueError"], nameof(LevelModel.Vol), nameof(LevelModel.Voh));

            if (entity.Iol > entity.Ioh)
                message = string.Format(L["ExceedValueError"], nameof(LevelModel.Iol), nameof(LevelModel.Ioh));

            if (entity.Vcl > entity.Vch)
                message = string.Format(L["ExceedValueError"], nameof(LevelModel.Vcl), nameof(LevelModel.Vch));

            if (!message.IsEmpty())
                throw new ArgumentException(message);

            var existPinOrGroup = await _repository.ExistsAsync(x => x.Id != entity.Id && x.LevelGroupId.Equals(entity.LevelGroupId) && x.GroupOrPinId.Equals(entity.GroupOrPinId));
            if (existPinOrGroup)
            {
                var name = string.Empty;
                var group = await _groupInfoRepository.FindByIdAsync(entity.GroupOrPinId);
                if (group == null)
                {
                    var pin = await _pinInfoRepository.FindByIdAsync(entity.GroupOrPinId);
                    name = pin?.PinName;
                }
                else
                    name = group?.GroupName;
                throw new ArgumentException(string.Format(L["FieldAlreadyExists"], name));
            }
            base.CreateBeforeAsync(entity);
        }
        #endregion

        #region 更新前事件
        protected override async Task UpdateBeforeAsync(Level entity)
        {
            var message = string.Empty;
            if (entity.Vil > entity.Vih)
                message = string.Format(L["ExceedValueError"], nameof(LevelModel.Vil), nameof(LevelModel.Vih));

            if (entity.Vol > entity.Voh)
                message = string.Format(L["ExceedValueError"], nameof(LevelModel.Vol), nameof(LevelModel.Voh));

            if (entity.Iol > entity.Ioh)
                message = string.Format(L["ExceedValueError"], nameof(LevelModel.Iol), nameof(LevelModel.Ioh));

            if (entity.Vcl > entity.Vch)
                message = string.Format(L["ExceedValueError"], nameof(LevelModel.Vcl), nameof(LevelModel.Vch));

            if (!message.IsEmpty())
                throw new ArgumentException(message);

            var existPinOrGroup = await _repository.ExistsAsync(x => x.Id != entity.Id && x.GroupOrPinId.Equals(entity.GroupOrPinId));
            if (existPinOrGroup)
            {
                var name = string.Empty;
                var group = await _groupInfoRepository.FindByIdAsync(entity.GroupOrPinId);
                if (group == null)
                {
                    var pin = await _pinInfoRepository.FindByIdAsync(entity.GroupOrPinId);
                    name = pin?.PinName;
                }
                else
                    name = group?.GroupName;
                throw new ArgumentException(string.Format(L["FieldAlreadyExists"], name));
            }
            base.CreateBeforeAsync(entity);
        }
        #endregion

    }
}
