using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;

namespace KSW.ATE01.Application.BLLs.Implements.TestPlans
{
    public class GroupInfoBLL : CrudServiceBase<GroupInfo>, IGroupInfoBLL
    {
        private readonly IGroupInfoRepository _repository;
        private readonly IPinGroupRelationshipRepositoy _pinGroupRelationshipRepositoy;

        public GroupInfoBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            IGroupInfoRepository repository,
            IPinGroupRelationshipRepositoy pinGroupRelationshipRepositoy) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
            _pinGroupRelationshipRepositoy = pinGroupRelationshipRepositoy;
        }

        public async Task<GroupInfoModel> GetByIdAsync(string id)
        {
            var result = await _repository.FindByIdAsync(id);
            return result?.MapTo<GroupInfoModel>();
        }

        public async Task<List<GroupInfoModel>> GetListByOverviewIdAsync(string overviewId)
        {
            var result = await _repository.FindAllAsync(x => x.PinOverviewId.Equals(overviewId.ToGuid()));
            return result?.MapToList<GroupInfoModel>();
        }

        public async Task<string> CreateAsync(GroupInfoModel model)
        {
            var entity = model.MapTo<GroupInfo>();
            await CreateAsync(entity);
            return entity.Id.SafeString();
        }

        public async Task<GroupInfoModel> UpdateAsync(GroupInfoModel model)
        {
            var entity = model.MapTo<GroupInfo>();
            await UpdateAsync(model.Id, entity);
            return await GetByIdAsync(model.Id);
        }

        public async Task DeleteWithDetailAsync(string id)
        {
            if (id.IsEmpty())
                return;

            var relationships = _pinGroupRelationshipRepositoy.FindAllAsync(x => x.GroupInfoId.Equals(id.ToGuid()));
            await _pinGroupRelationshipRepositoy.RemoveAsync(await relationships);
            await _repository.RemoveAsync(id);
            await CommitAsync();
        }
    }
}
