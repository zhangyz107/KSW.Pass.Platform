using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Data.Repositories.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;

namespace KSW.ATE01.Application.BLLs.Implements.TestPlans
{
    public class PinInfoBLL : CrudServiceBase<PinInfo>, IPinInfoBLL
    {
        private readonly IPinInfoRepository _repository;
        private readonly IPinSiteInfoRepository _pinSiteInfoRepository;
        private readonly IGroupInfoRepository _groupInfoRepository;
        private readonly IPinGroupRelationshipRepositoy _pinGroupRelationshipRepositoy;

        public PinInfoBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            IPinInfoRepository repository,
            IPinSiteInfoRepository pinSiteInfoRepository,
            IGroupInfoRepository groupInfoRepository,
            IPinGroupRelationshipRepositoy pinGroupRelationshipRepositoy) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
            _pinSiteInfoRepository = pinSiteInfoRepository;
            _groupInfoRepository = groupInfoRepository;
            _pinGroupRelationshipRepositoy = pinGroupRelationshipRepositoy;
        }

        public async Task<List<PinInfoModel>> GetPinInfosFromOvewviewIdAsync(string overviewId, List<string> ids = null)
        {
            if (overviewId.IsEmpty())
                return null;

            var pinInfos = await _repository?.FindAllAsync(x => x.PinOverviewId.Equals(overviewId.ToGuid()));
            if (!ids.IsEmpty())
                pinInfos = pinInfos.Where(x => !ids.Contains(x.Id.SafeString())).ToList();

            var result = pinInfos?.MapToList<PinInfoModel>();
            if (!result.IsEmpty())
            {
                var pinIds = pinInfos.Select(x => x.Id).ToList();
                #region 获得引脚站点信息
                var pinSites = await _pinSiteInfoRepository.FindAllAsync(x => pinIds.Contains(x.PinInfoId));
                var index = 0;
                foreach (var item in result)
                {
                    var channelNames = pinSites.Where(x => x.PinInfoId.SafeString()?.Equals(item.Id) == true).Select(y => y.ChannelName);
                    item.SortId = ++index;
                    item.ChannelName = string.Join(", ", channelNames);
                }
                #endregion

                #region 获得引脚和组信息
                var pinGroups = await _pinGroupRelationshipRepositoy.FindAllAsync(x => pinIds.Contains(x.PinInfoId));
                var groupIds = pinGroups.Select(x => x.GroupInfoId).Distinct().ToList();
                var groups = await _groupInfoRepository.FindAllAsync(x => groupIds.Contains(x.Id));
                foreach (var item in result)
                {
                    var itemGroupIds = pinGroups.Where(x => x.PinInfoId.Equals(item.Id.ToGuid())).Select(x => x.GroupInfoId);
                    var itemGroupsName = groups.Where(x => itemGroupIds.Contains(x.Id)).OrderBy(x => x.CreationTime).Select(x => x.GroupName);
                    item.GroupName = string.Join(",", itemGroupsName);
                }
                #endregion

            }

            return result;
        }

        public async Task<string> SaveAsync(PinInfoModel model)
        {
            if (model == null)
                return null;

            var entity = model.MapTo<PinInfo>();
            if (model.IsNew)
            {
                entity.Init();
                await CreateAsync(entity);
            }
            else
            {
                await UpdateAsync(model.Id, entity);
            }

            return entity.Id.SafeString();
        }

        public async Task DeletePinAndDetailsByIdAsync(string id)
        {
            var pinSiteInfos = await _pinSiteInfoRepository.FindAllAsync(x => x.PinInfoId.Equals(id.ToGuid()));
            await _pinSiteInfoRepository.RemoveAsync(pinSiteInfos);
            await _repository.RemoveAsync(id);
            await CommitAsync();
        }
    }
}
