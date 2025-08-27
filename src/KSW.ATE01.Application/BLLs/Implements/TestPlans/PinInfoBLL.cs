using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Data.Repositories.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Results;
using NPOI.SS.Formula.Atp;

namespace KSW.ATE01.Application.BLLs.Implements.TestPlans
{
    public class PinInfoBLL : CrudServiceBase<PinInfo>, IPinInfoBLL
    {
        private readonly IPinInfoRepository _repository;
        private readonly ISiteInfoRepository _siteInfoRepository;
        private readonly IPinSiteInfoRepository _pinSiteInfoRepository;
        private readonly IGroupInfoRepository _groupInfoRepository;
        private readonly IPinGroupRelationshipRepositoy _pinGroupRelationshipRepositoy;
        private readonly IPinChannelManager _pinChannelManager;

        public PinInfoBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            IPinInfoRepository repository,
            ISiteInfoRepository siteInfoRepository,
            IPinSiteInfoRepository pinSiteInfoRepository,
            IGroupInfoRepository groupInfoRepository,
            IPinGroupRelationshipRepositoy pinGroupRelationshipRepositoy,
            IPinChannelManager pinChannelManager) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
            _siteInfoRepository = siteInfoRepository;
            _pinSiteInfoRepository = pinSiteInfoRepository;
            _groupInfoRepository = groupInfoRepository;
            _pinGroupRelationshipRepositoy = pinGroupRelationshipRepositoy;
            _pinChannelManager = pinChannelManager;
        }

        public async Task<PinInfoModel> GetByIdAsync(string id)
        {
            var entity = await _repository?.FindByIdAsync(id);
            var model = entity?.MapTo<PinInfoModel>();
            await GetDetailAsync(model);
            return model;
        }

        private async Task GetDetailAsync(PinInfoModel? model)
        {
            if (model == null)
                return;

            var pinSites = await _pinSiteInfoRepository.FindAllAsync(x => x.PinInfoId.Equals(model.Id.ToGuid()));
            var siteIds = pinSites.Select(x => x.SiteInfoId).Distinct().ToList();
            var sites = await _siteInfoRepository.FindAllAsync(x => siteIds.Contains(x.Id));
            model.PinSiteInfos = pinSites.OrderBy(x => x.SortId).MapToList<PinSiteInfoModel>();
            foreach (var pinSite in model.PinSiteInfos)
            {
                var site = sites.FirstOrDefault(x => x.Id.Equals(pinSite.SiteInfoId));
                pinSite.SiteName = site?.SiteName;
            }
            model.ChannelName = string.Join(", ", pinSites.OrderBy(x => x.SortId).Select(x => x.ChannelName));

            var pinGroups = await _pinGroupRelationshipRepositoy.FindAllAsync(x => x.PinInfoId.Equals(model.Id.ToGuid()));
            var groupIds = pinGroups.Select(x => x.GroupInfoId).Distinct().ToList();
            var groups = await _groupInfoRepository.FindAllAsync(x => groupIds.Contains(x.Id));
            model.GroupName = string.Join(", ", groups.Select(x => x.GroupName));
        }

        public async Task<string> CreateAsync(PinInfoModel model)
        {
            var id = await _pinChannelManager?.CreatePinAndSiteInfoAsync(model);
            await CommitAsync();
            return id;
        }

        public async Task<PinInfoModel> UpdateAsync(PinInfoModel model)
        {
            await _pinChannelManager?.UpdatePinAndSiteInfoAsync(model);
            await CommitAsync();
            return await GetByIdAsync(model.Id);
        }

        public async Task DeleteAsync(string id)
        {
            await _pinChannelManager?.DeletePinAndSiteInfoByIdAsync(id);
            await CommitAsync();
        }

        public async Task<List<PinInfoModel>> GetPinInfosFromOvewviewIdAsync(string overviewId, List<string> ids = null)
        {
            if (overviewId.IsEmpty())
                return null;

            var pinInfos = await _repository?.FindAllAsync(x => x.PinOverviewId.Equals(overviewId.ToGuid()));
            if (!ids.IsEmpty())
                pinInfos = pinInfos.Where(x => !ids.Contains(x.Id.SafeString())).ToList();

            var result = pinInfos?.MapToList<PinInfoModel>();
            await GetDetailAsync(result);
            //if (!result.IsEmpty())
            //{
            //    var pinIds = pinInfos.Select(x => x.Id).ToList();
            //    #region 获得引脚站点信息
            //    var pinSites = await _pinSiteInfoRepository.FindAllAsync(x => pinIds.Contains(x.PinInfoId));
            //    var index = 0;
            //    foreach (var item in result)
            //    {
            //        var channelNames = pinSites.Where(x => x.PinInfoId.SafeString()?.Equals(item.Id) == true).OrderBy(x => x.SortId).Select(y => y.ChannelName);
            //        item.SortId = ++index;
            //        item.ChannelName = string.Join(", ", channelNames);
            //    }
            //    #endregion

            //    #region 获得引脚和组信息
            //    var pinGroups = await _pinGroupRelationshipRepositoy.FindAllAsync(x => pinIds.Contains(x.PinInfoId));
            //    var groupIds = pinGroups.Select(x => x.GroupInfoId).Distinct().ToList();
            //    var groups = await _groupInfoRepository.FindAllAsync(x => groupIds.Contains(x.Id));
            //    foreach (var item in result)
            //    {
            //        var itemGroupIds = pinGroups.Where(x => x.PinInfoId.Equals(item.Id.ToGuid())).Select(x => x.GroupInfoId);
            //        var itemGroupsName = groups.Where(x => itemGroupIds.Contains(x.Id)).OrderBy(x => x.CreationTime).Select(x => x.GroupName);
            //        if (itemGroupsName.Any())
            //            item.GroupName = string.Join(",", itemGroupsName);
            //    }
            //    #endregion

            //}

            return result;
        }

        private async Task GetDetailAsync(List<PinInfoModel> models)
        {
            if (models.IsEmpty())
                return;

            foreach (var model in models)
                await GetDetailAsync(model);
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

        #region 创建前事件
        protected override async Task CreateBeforeAsync(PinInfo entity)
        {
            var message = string.Empty;
            var existPin = await _repository.ExistsAsync(x => x.Id != entity.Id && x.PinOverviewId.Equals(entity.PinOverviewId) && x.PinName.Equals(entity.PinName));
            if (existPin)
                message = string.Format(L["FieldAlreadyExists"], L["PinName"]);

            if (!message.IsEmpty())
                throw new ArgumentException(message);

            base.CreateBeforeAsync(entity);
        }
        #endregion

        #region 更新前事件
        protected override async Task UpdateBeforeAsync(PinInfo entity)
        {
            var message = string.Empty;
            var existPin = await _repository.ExistsAsync(x => x.Id != entity.Id && x.PinOverviewId.Equals(entity.PinOverviewId) && x.PinName.Equals(entity.PinName));
            if (existPin)
                message = string.Format(L["FieldAlreadyExists"], L["PinName"]);

            if (!message.IsEmpty())
                throw new ArgumentException(message);

            base.UpdateBeforeAsync(entity);
        }
        #endregion
    }
}
