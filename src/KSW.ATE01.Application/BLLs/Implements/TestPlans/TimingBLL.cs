/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：TimingBLL.cs
// 功能描述：时钟逻辑层
//
// 作者：zhangyingzhong
// 日期：2025/08/21 10:43
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using System.Windows.Media;

namespace KSW.ATE01.Application.BLLs.Implements.TestPlans
{
    /// <summary>
    /// 时钟逻辑层
    /// </summary>
    public class TimingBLL : CrudServiceBase<Timing>, ITimingBLL
    {
        private readonly ITimingRepository _repository;
        private readonly ITimingGroupRepository _timingGroupRepository;
        private readonly IGroupInfoRepository _groupInfoRepository;
        private readonly IPinInfoRepository _pinInfoRepository;

        public TimingBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            ITimingRepository repository,
            ITimingGroupRepository timingGroupRepository,
            IGroupInfoRepository groupInfoRepository,
            IPinInfoRepository pinInfoRepository) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
            _timingGroupRepository = timingGroupRepository;
            _groupInfoRepository = groupInfoRepository;
            _pinInfoRepository = pinInfoRepository;
        }

        public Task<TimingModel> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TimingModel>> GetListByGroupIdAsync(string groupId)
        {
            var list = await _repository.FindAllAsync(x => x.TimingGroupId.Equals(groupId.ToGuid()));
            var timingGroupIds = list.Select(x => x.TimingGroupId).Distinct().ToList();
            var timingGroups = await _timingGroupRepository.FindAllAsync(x => timingGroupIds.Contains(x.Id));
            var ids = list.Select(x => x.GroupOrPinId).Distinct().ToList();
            var groups = await _groupInfoRepository.FindAllAsync(x => ids.Contains(x.Id));
            var pins = await _pinInfoRepository.FindAllAsync(x => ids.Contains(x.Id));
            var result = list?.MapToList<TimingModel>();

            foreach (var item in result)
            {
                item.TimingGroupName = timingGroups.FirstOrDefault(x => x.Id.Equals(item.TimingGroupId))?.TimingGroupName;
                item.PinOrGroupName = groups.FirstOrDefault(x => x.Id.Equals(item.GroupOrPinId))?.GroupName;
                if (item.PinOrGroupName.IsEmpty())
                    item.PinOrGroupName = pins.FirstOrDefault(x => x.Id.Equals(item.GroupOrPinId))?.PinName;
            }

            return result;
        }

        public async Task<string> CreateAsync(TimingModel model)
        {
            var entity = model.MapTo<Timing>();
            await CreateAsync(entity);
            return entity.Id.SafeString();
        }

        public async Task<TimingModel> UpdateAsync(TimingModel model)
        {
            var entity = model.MapTo<Timing>();
            await UpdateAsync(model.Id, entity);
            return await GetByIdAsync(model.Id);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.RemoveAsync(id);
            await CommitAsync();
        }

        #region 创建时事件
        protected override async Task CreateBeforeAsync(Timing entity)
        {
            var message = string.Empty;

            if (entity.Period < entity.DriveA)
                message = string.Format(L["ExceedValueError"], nameof(TimingModel.DriveA), nameof(TimingModel.Period));

            if (entity.Period < entity.DriveB)
                message = string.Format(L["ExceedValueError"], nameof(TimingModel.DriveB), nameof(TimingModel.Period));

            if (entity.Period < entity.DriveC)
                message = string.Format(L["ExceedValueError"], nameof(TimingModel.DriveC), nameof(TimingModel.Period));

            if (entity.Period < entity.DriveD)
                message = string.Format(L["ExceedValueError"], nameof(TimingModel.DriveD), nameof(TimingModel.Period));

            if (entity.Period < entity.StrobeA)
                message = string.Format(L["ExceedValueError"], nameof(TimingModel.StrobeA), nameof(TimingModel.Period));

            if (entity.Period < entity.StrobeB)
                message = string.Format(L["ExceedValueError"], nameof(TimingModel.StrobeB), nameof(TimingModel.Period));

            if (!message.IsEmpty())
                throw new ArgumentException(message);

            await base.CreateBeforeAsync(entity);
        }
        #endregion
    }
}
