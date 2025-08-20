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
    /// 门限业务逻辑
    /// </summary>
    public class LimitsBLL : CrudServiceBase<Limits>, ILimitsBLL
    {
        private readonly ILimitsRepository _repository;

        public LimitsBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            ILimitsRepository repository) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
        }

        public async Task<LimitsModel> GetByIdAsync(string id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return entity?.MapTo<LimitsModel>();
        }

        public async Task<List<LimitsModel>> GetListByProjectIdAsync(string projectId)
        {
            var list = await _repository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
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
            await _repository.RemoveAsync(id);
            await CommitAsync();
            return;
        }
    }
}
