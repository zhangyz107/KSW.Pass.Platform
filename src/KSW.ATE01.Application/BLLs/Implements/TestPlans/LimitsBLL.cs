/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：LimitsBLL.cs
// 功能描述：门限业务逻辑
//
// 作者：zhangyingzhong
// 日期：2025/08/19 15:03
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

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
    /// 门限业务逻辑
    /// </summary>
    public class LimitsBLL : CrudServiceBase<Limits>, ILimitsBLL
    {
        private readonly ILimitsRepository _repository;
        private readonly ILimitsManager _limitsManager;

        public LimitsBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            ILimitsRepository repository,
            ILimitsManager limitsManager) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
            _limitsManager = limitsManager;
        }

        public async Task<LimitsModel> GetByIdAsync(string id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return entity?.MapTo<LimitsModel>();
        }

        public async Task<List<LimitsModel>> GetListByProjectIdAsync(string projectId)
        {
            var list = (await _repository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid())))?.OrderBy(x=>x.CreationTime);
            return list?.MapToList<LimitsModel>();
        }

        public async Task<string> CreateAsync(LimitsModel model)
        {
            var entity = model.MapTo<Limits>();
            await CreateAsync(entity);
            return entity.Id.SafeString();
        }

        public async Task<LimitsModel> UpdateAsync(LimitsModel model)
        {
            var entity = model.MapTo<Limits>();
            await UpdateAsync(model.Id, entity);
            return await GetByIdAsync(model.Id);
        }

        public async Task DeleteAsync(string id)
        {
            var entities = await _repository.FindByIdsAsync(id);
            await DeleteBeforeAsync(entities);
            await _repository.RemoveAsync(id);
            await CommitAsync();
        }

        #region 创建前事件
        protected override async Task CreateBeforeAsync(Limits entity)
        {
            var message = string.Empty;
            if (entity.LowLimit > entity.HighLimit)
                message = L["LimitValueError"];

            var existLimit = await _repository.FindAllAsync(x => x.Id != entity.Id && x.ProjectInfoId.Equals(entity.ProjectInfoId) && x.LimitName.Equals(entity.LimitName));
            if (!existLimit.IsEmpty())
                message = string.Format(L["FieldAlreadyExists"], entity.LimitName);

            var limits = await _repository.FindAllAsync(x => x.ProjectInfoId.Equals(entity.ProjectInfoId));
        }
        #endregion

        #region 更新前事件
        protected override async Task UpdateBeforeAsync(Limits entity)
        {
            var message = string.Empty;
            if (entity.LowLimit > entity.HighLimit)
                message = L["LimitValueError"];

            var existLimit = await _repository.FindAllAsync(x => x.Id != entity.Id && x.ProjectInfoId.Equals(entity.ProjectInfoId) && x.LimitName.Equals(entity.LimitName));
            if (!existLimit.IsEmpty())
                message = string.Format(L["FieldAlreadyExists"], entity.LimitName);
        }
        #endregion

        #region 删除前事件
        private async Task DeleteBeforeAsync(List<Limits> entities)
        {
            await _limitsManager?.ValidateDeleteAsync(entities);
        }
        #endregion
    }
}
