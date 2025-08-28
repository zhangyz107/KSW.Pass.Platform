using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
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
        private readonly IGroupInfoManager _groupInfoManager;

        public GroupInfoBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            IGroupInfoRepository repository,
            IPinGroupRelationshipRepositoy pinGroupRelationshipRepositoy,
            IGroupInfoManager groupInfoManager) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
            _pinGroupRelationshipRepositoy = pinGroupRelationshipRepositoy;
            _groupInfoManager = groupInfoManager;
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

            var entities = await _repository.FindByIdsAsync(id);
            await DeleteBeforeAsync(entities);

            var relationships = _pinGroupRelationshipRepositoy.FindAllAsync(x => x.GroupInfoId.Equals(id.ToGuid()));
            await _pinGroupRelationshipRepositoy.RemoveAsync(await relationships);
            await _repository.RemoveAsync(id);
            await CommitAsync();
        }

        #region 创建时事件
        protected override async Task CreateBeforeAsync(GroupInfo entity)
        {
            if (entity.GroupName.IsEmpty())
            {
                throw new ArgumentNullException($"{L["GroupName"]}{L["CanNotBeEmpty"]}");
            }

            var isExist = await _repository.ExistsAsync(x => x.PinOverviewId.Equals(entity.PinOverviewId) && x.Id != entity.Id && x.GroupName.Equals(entity.GroupName));
            if (isExist)
            {
                throw new ArgumentException(string.Format(L["FieldAlreadyExists"], entity.GroupName));
            }

            var groupInfos = await _repository.FindAllAsync(x => x.PinOverviewId.Equals(entity.PinOverviewId));

            base.CreateBeforeAsync(entity);
        }
        #endregion

        #region 更新时事件
        protected override async Task UpdateBeforeAsync(GroupInfo entity)
        {
            if (entity.GroupName.IsEmpty())
            {
                throw new ArgumentNullException($"{L["GroupName"]}{L["CanNotBeEmpty"]}");
            }

            var isExist = await _repository.ExistsAsync(x => x.PinOverviewId.Equals(entity.PinOverviewId) && x.Id != entity.Id && x.GroupName.Equals(entity.GroupName));
            if (isExist)
            {
                throw new ArgumentException(string.Format(L["FieldAlreadyExists"], entity.GroupName));
            }
            base.UpdateBeforeAsync(entity);
        }
        #endregion

        #region 删除前事件
        private async Task DeleteBeforeAsync(List<GroupInfo> entities)
        {
            await _groupInfoManager?.ValidateDeleteAsync(entities);
        }
        #endregion
    }
}
