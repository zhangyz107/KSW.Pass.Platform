
/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：GlobalParameterBLL.cs
// 功能描述：全局参数逻辑层
//
// 作者：zhangyingzhong
// 日期：2025/08/25 10:34
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
    /// 全局参数逻辑层
    /// </summary>
    public class GlobalParameterBLL : CrudServiceBase<GlobalParameter>, IGlobalParameterBLL
    {
        #region Fields
        private readonly IGlobalParameterRepository _repository;
        #endregion

        public GlobalParameterBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            IGlobalParameterRepository repository) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
        }

        public async Task<GlobalParameterModel> GetByIdAsync(string id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return entity?.MapTo<GlobalParameterModel>();
        }

        public async Task<List<GlobalParameterModel>> GetListByProjectIdAsync(string projectId)
        {
            var list = (await _repository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid())))?.OrderBy(x => x.CreationTime);
            return list.MapToList<GlobalParameterModel>();
        }

        public async Task<string> CreateAsync(GlobalParameterModel model)
        {
            var entity = model.MapTo<GlobalParameter>();
            await CreateAsync(entity);
            return entity.Id.SafeString();
        }

        public async Task<GlobalParameterModel> UpdateAsync(GlobalParameterModel model)
        {
            var entity = model.MapTo<GlobalParameter>();
            await UpdateAsync(model.Id, entity);
            return await GetByIdAsync(model.Id);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.RemoveAsync(id);
            await CommitAsync();
        }

        #region 创建前事件
        protected override async Task CreateBeforeAsync(GlobalParameter entity)
        {
            var exist = await _repository.ExistsAsync(x => x.Id != entity.Id && x.ProjectInfoId.Equals(entity.ProjectInfoId) && x.PatternFile.Equals(entity.PatternFile));
            if (exist)
            {
                throw new ArgumentException(string.Format(L["FieldAlreadyExists"], entity.PatternFile));
            }

            var globalParameters = await _repository.FindAllAsync(x => x.ProjectInfoId.Equals(entity.ProjectInfoId));
        }
        #endregion

        #region 更新前事件
        protected override async Task UpdateBeforeAsync(GlobalParameter entity)
        {
            var exist = await _repository.ExistsAsync(x => x.Id != entity.Id && x.ProjectInfoId.Equals(entity.ProjectInfoId) && x.PatternFile.Equals(entity.PatternFile));
            if (exist)
            {
                throw new ArgumentException(string.Format(L["FieldAlreadyExists"], entity.PatternFile));
            }
        }
        #endregion

    }
}
