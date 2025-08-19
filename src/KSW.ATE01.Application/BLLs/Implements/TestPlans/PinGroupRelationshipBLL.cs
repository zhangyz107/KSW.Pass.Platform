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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.BLLs.Implements.TestPlans
{
    public class PinGroupRelationshipBLL : CrudServiceBase<PinGroupRelationship>, IPinGroupRelationshipBLL
    {
        private readonly IPinGroupRelationshipRepositoy _repositoy;
        private readonly IGroupInfoRepository _groupInfoRepository;
        private readonly IPinInfoRepository _pinInfoRepository;

        public PinGroupRelationshipBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            IPinGroupRelationshipRepositoy repository,
            IGroupInfoRepository groupInfoRepository,
            IPinInfoRepository pinInfoRepository) : base(containerProvider, unitOfWork, repository)
        {
            _repositoy = repository;
            _groupInfoRepository = groupInfoRepository;
            _pinInfoRepository = pinInfoRepository;
        }

        public async Task<PinGroupRelationshipModel> GetByIdAsync(string id)
        {
            var entity = await _repositoy.FindByIdAsync(id);
            var result = entity?.MapTo<PinGroupRelationshipModel>();
            await GetDetailAsync(result);
            return result;
        }

        public async Task<List<PinGroupRelationshipModel>> GetListByGroupIdAsync(string groupId)
        {
            var entities = await _repositoy.FindAllAsync(x => x.GroupInfoId.Equals(groupId.ToGuid()));
            var result = entities.MapToList<PinGroupRelationshipModel>();
            await GetDetailAsync(result);
            return result;
        }

        public async Task<List<PinGroupRelationshipModel>> GetListByPinIdAsync(string pinId)
        {
            var entities = await _repositoy.FindAllAsync(x => x.PinInfoId.Equals(pinId.ToGuid()));
            var result = entities.MapToList<PinGroupRelationshipModel>();
            await GetDetailAsync(result);
            return result;
        }

        public async Task<string> SaveAsync(PinGroupRelationshipModel model)
        {
            var entity = model.MapTo<PinGroupRelationship>();
            if (model.IsNew)
                await CreateAsync(entity);
            else
                await UpdateAsync(model.Id, entity);

            return model.Id ?? entity.Id.SafeString();
        }

        public async Task DeleteAsync(string id)
        {
            await _repositoy.RemoveAsync(id);
            await CommitAsync();
        }

        private async Task GetDetailAsync(PinGroupRelationshipModel? result)
        {
            if (result == null)
                return;

            var pinInfo = await _pinInfoRepository.FindByIdAsync(result?.PinInfoId);
            var groupInfo = await _groupInfoRepository.FindByIdAsync(result?.GroupInfoId);
            result.PinName = pinInfo?.PinName;
            result.GroupName = groupInfo?.GroupName;
        }

        private async Task GetDetailAsync(List<PinGroupRelationshipModel> result)
        {
            if (result.IsEmpty())
                return;

            var pinIds = result.Select(x => x.PinInfoId).ToList();
            var pinInfos = await _pinInfoRepository.FindAllAsync(x => pinIds.Contains(x.Id));
            var groupIds = result.Select(x => x.GroupInfoId).ToList();
            var groupInfos = await _groupInfoRepository.FindAllAsync(x => groupIds.Contains(x.Id));
            foreach (var item in result)
            {
                var pinInfo = pinInfos.FirstOrDefault(x => x.Id.Equals(item.PinInfoId));
                var groupInfo = groupInfos.FirstOrDefault(x => x.Id.Equals(item.GroupInfoId));
                item.PinName = pinInfo?.PinName;
                item.GroupName = groupInfo?.GroupName;
            }
            return;
        }
    }
}
