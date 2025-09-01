/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：TestItemInfoBLL.cs
// 功能描述：测试项信息逻辑层
//
// 作者：zhangyingzhong
// 日期：2025/08/21 15:33
// 修改记录(Revision History)
//
//------------------------------------------------------------*/
using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;

namespace KSW.ATE01.Application.BLLs.Implements.TestPlans
{
    /// <summary>
    /// 测试项信息逻辑层
    /// </summary>
    public class TestItemInfoBLL : CrudServiceBase<TestItemInfo>, ITestItemInfoBLL
    {
        private readonly ITestItemInfoRepository _repository;
        private readonly IGroupInfoRepository _groupInfoRepository;
        private readonly IPinInfoRepository _pinInfoRepository;
        private readonly ILimitsRepository _limitsRepository;
        private readonly ILevelGroupRepository _levelGroupRepository;
        private readonly ITimingGroupRepository _timingGroupRepository;

        public TestItemInfoBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            ITestItemInfoRepository repository,
            IGroupInfoRepository groupInfoRepository,
            IPinInfoRepository pinInfoRepository,
            ILimitsRepository limitsRepository,
            ILevelGroupRepository levelGroupRepository,
            ITimingGroupRepository timingGroupRepository) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
            _groupInfoRepository = groupInfoRepository;
            _pinInfoRepository = pinInfoRepository;
            _limitsRepository = limitsRepository;
            _levelGroupRepository = levelGroupRepository;
            _timingGroupRepository = timingGroupRepository;
        }

        public async Task<TestItemInfoModel> GetByIdAsync(string id)
        {
            var entity = await _repository.FindByIdAsync(id);
            var model = entity?.MapTo<TestItemInfoModel>();
            await GetDetailAsync(model);
            return model;
        }

        public async Task<List<TestItemInfoModel>> GetListByProjectIdAsync(string projectId)
        {
            var list = (await _repository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid())))?.OrderBy(x => x.CreationTime);
            var models = list?.MapToList<TestItemInfoModel>();
            await GetDetailAsync(models);
            return models;
        }

        public async Task<string> CreateAsync(TestItemInfoModel model)
        {
            var entity = model.MapTo<TestItemInfo>();
            await CreateAsync(entity);
            return entity.Id.SafeString();
        }

        public async Task<TestItemInfoModel> UpdateAsync(TestItemInfoModel model)
        {
            var entity = model.MapTo<TestItemInfo>();
            await UpdateAsync(model.Id, entity);
            return await GetByIdAsync(model.Id);
        }

        public async Task SaveAsync(List<TestItemInfoModel> models)
        {
            var enetities = models.MapToList<TestItemInfo>();
            foreach (var entity in enetities)
                await _repository.UpdateAsync(entity);

            await CommitAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.RemoveAsync(id);
            await CommitAsync();
        }

        private async Task GetDetailAsync(TestItemInfoModel model)
        {
            if (model == null)
                return;

            var groupInfo = await _groupInfoRepository.FindByIdAsync(model?.GroupOrPinId);
            model.PinOrGroupName = groupInfo?.GroupName;
            if (model.PinOrGroupName.IsEmpty())
            {
                var pinInfo = await _pinInfoRepository.FindByIdAsync(model?.GroupOrPinId);
                model.PinOrGroupName = pinInfo?.PinName;
            }

            var limit = await _limitsRepository.FindByIdAsync(model?.LimitsId);
            model.LimitName = limit?.LimitName;

            var levelGroup = await _levelGroupRepository.FindByIdAsync(model?.LevelGroupId);
            model.LevelGroupName = levelGroup?.LevelGroupName;

            var timingGroup = await _timingGroupRepository.FindByIdAsync(model?.TimingGroupId);
            model.TimingGroupName = timingGroup?.TimingGroupName;
        }

        private async Task GetDetailAsync(List<TestItemInfoModel> models)
        {
            if (models.IsEmpty())
                return;

            foreach (var model in models)
                await GetDetailAsync(model);
        }

        #region 创建前事件
        protected override async Task CreateBeforeAsync(TestItemInfo entity)
        {
            var existTestItemName = await _repository.ExistsAsync(x => x.Id != entity.Id && x.ProjectInfoId.Equals(entity.ProjectInfoId) && x.TestItemName.Equals(entity.TestItemName));
            if (existTestItemName)
            {
                throw new ArgumentException(string.Format(L["FieldAlreadyExists"], $"{L["TestItemName"]}:{entity.TestItemName}"));
            }

            var existFunctionName = await _repository.ExistsAsync(x => x.Id != entity.Id && x.ProjectInfoId.Equals(entity.ProjectInfoId) && x.FunctionName.Equals(entity.FunctionName));
            if (existFunctionName)
            {
                throw new ArgumentException(string.Format(L["FieldAlreadyExists"], $"{L["FunctionName"]}:{entity.FunctionName}"));
            }

            var testItems = await _repository.FindAllAsync(x => x.ProjectInfoId.Equals(entity.ProjectInfoId));

            await base.CreateBeforeAsync(entity);
        }
        #endregion

        #region 更新前事件
        protected override async Task UpdateBeforeAsync(TestItemInfo entity)
        {
            var existTestItemName = await _repository.ExistsAsync(x => x.Id != entity.Id && x.ProjectInfoId.Equals(entity.ProjectInfoId) && x.TestItemName.Equals(entity.TestItemName));
            if (existTestItemName)
            {
                throw new ArgumentException(string.Format(L["FieldAlreadyExists"], $"{L["TestItemName"]}:{entity.TestItemName}"));
            }

            var existFunctionName = await _repository.ExistsAsync(x => x.Id != entity.Id && x.ProjectInfoId.Equals(entity.ProjectInfoId) && x.FunctionName.Equals(entity.FunctionName));
            if (existFunctionName)
            {
                throw new ArgumentException(string.Format(L["FieldAlreadyExists"], $"{L["FunctionName"]}:{entity.FunctionName}"));
            }

            await base.UpdateBeforeAsync(entity);
        }
        #endregion

    }
}
