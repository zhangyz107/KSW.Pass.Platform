using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;

namespace KSW.ATE01.Application.BLLs.Implements.TestPlans
{
    /// <summary>
    /// 引脚站点业务逻辑接口
    /// </summary>
    public class PinSiteInfoBLL : CrudServiceBase<PinSiteInfo>, IPinSiteInfoBLL
    {
        private readonly IPinSiteInfoRepository _repository;
        private readonly IPinInfoRepository _pinInfoRepository;
        private readonly ISiteInfoRepository _siteInfoRepository;

        public PinSiteInfoBLL(
            IContainerProvider containerProvider,
            ISystemUnitOfWork unitOfWork,
            IPinSiteInfoRepository repository,
            IPinInfoRepository pinInfoRepository,
            ISiteInfoRepository siteInfoRepository) : base(containerProvider, unitOfWork, repository)
        {
            _repository = repository;
            _pinInfoRepository = pinInfoRepository;
            _siteInfoRepository = siteInfoRepository;
        }

        public async Task<List<PinSiteInfoModel>> GetAllPinSiteByOverviewIdAsync(string overviewId)
        {
            var result = new List<PinSiteInfoModel>();
            if (overviewId.IsEmpty())
                return result;

            var siteInfos = await _siteInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(overviewId.ToGuid()));

            foreach (var siteInfo in siteInfos)
            {
                var tempList = await _repository.FindAllAsync(x => x.SiteInfoId.Equals(siteInfo.Id));
                var pinSiteInfos = tempList?.MapToList<PinSiteInfoModel>();
                foreach (var item in pinSiteInfos)
                {
                    item.SiteName = siteInfo.SiteName;
                }
                result.AddRange(pinSiteInfos);
            }

            return result;
        }

        public async Task<List<PinSiteInfoModel>> GetPinSiteByPinIdAsync(string pinId)
        {
            var result = new List<PinSiteInfoModel>();
            if (pinId.IsEmpty())
                return result;

            var pinSiteInfos = await _repository.FindAllAsync(x => x.PinInfoId.Equals(pinId.ToGuid()));
            result = pinSiteInfos?.MapToList<PinSiteInfoModel>();

            var pinInfo = await _pinInfoRepository.FindByIdAsync(pinId);
            if (pinInfo != null)
            {
                var siteInfos = await _siteInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(pinInfo.PinOverviewId));
                foreach (var item in result)
                {
                    item.SiteName = siteInfos?.FirstOrDefault(x => x.Id.Equals(item.SiteInfoId))?.SiteName;
                }
            }

            return result;
        }

        public async Task SaveAsync(List<PinSiteInfoModel> createList, List<PinSiteInfoModel> updateList, List<PinSiteInfoModel> deleteList)
        {
            createList ??= new List<PinSiteInfoModel>();
            updateList ??= new List<PinSiteInfoModel>();
            deleteList ??= new List<PinSiteInfoModel>();

            var ceateEntity = createList.MapToList<PinSiteInfo>();
            var updateEntity = updateList.MapToList<PinSiteInfo>();
            var deleteEntity = deleteList.MapToList<PinSiteInfo>();

            await base.SaveAsync(ceateEntity, updateEntity, deleteEntity);
        }
    }
}
