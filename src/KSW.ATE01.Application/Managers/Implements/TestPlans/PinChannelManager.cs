
/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：PinChannelManager.cs
// 功能描述：引脚通道管理
//
// 作者：zhangyingzhong
// 日期：2025/08/22 15:20
// 修改记录(Revision History)
//
//------------------------------------------------------------*/


using KSW.Application;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data.Repositories.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;

namespace KSW.ATE01.Application.Managers.Implements.TestPlans
{
    /// <summary>
    /// 引脚通道管理
    /// </summary>
    public class PinChannelManager : ServiceBase, IPinChannelManager
    {
        #region Fields
        private readonly IPinInfoRepository _pinInfoRepository;
        private readonly IPinSiteInfoRepository _pinSiteInfoRepository;
        private readonly IPinGroupRelationshipRepositoy _pinGroupRelationshipRepositoy;
        private readonly ILevelRepository _levelRepository;
        private readonly ITimingRepository _timingRepository;
        private readonly ITestItemInfoRepository _testItemInfoRepository;
        #endregion

        public PinChannelManager(
            IContainerProvider containerProvider,
            IPinInfoRepository pinInfoRepository,
            IPinSiteInfoRepository pinSiteInfoRepository,
            IPinGroupRelationshipRepositoy pinGroupRelationshipRepositoy,
            ILevelRepository levelRepository,
            ITimingRepository timingRepository,
            ITestItemInfoRepository testItemInfoRepository) : base(containerProvider)
        {
            _pinInfoRepository = pinInfoRepository;
            _pinSiteInfoRepository = pinSiteInfoRepository;
            _pinGroupRelationshipRepositoy = pinGroupRelationshipRepositoy;
            _levelRepository = levelRepository;
            _timingRepository = timingRepository;
            _testItemInfoRepository = testItemInfoRepository;
        }

        public async Task<string> CreatePinAndSiteInfoAsync(PinInfoModel pinInfo)
        {
            var pinSiteInfos = pinInfo?.PinSiteInfos;
            await CreatePinInfoBeforeAsync(pinInfo);
            await CreatePinSiteInfoBeforeAsync(pinSiteInfos);

            var pinInfoEntity = pinInfo.MapTo<PinInfo>();
            pinInfoEntity.Init();
            await _pinInfoRepository.AddAsync(pinInfoEntity);

            var pinSiteInfoEntities = pinSiteInfos.MapToList<PinSiteInfo>();
            var index = 0;
            foreach (var pinSiteInfoEntity in pinSiteInfoEntities)
            {
                pinSiteInfoEntity.Init();
                pinSiteInfoEntity.SortId = index++;
                await _pinSiteInfoRepository.AddAsync(pinSiteInfoEntity);
            }

            return pinInfoEntity.Id.ToString();
        }

        public async Task UpdatePinAndSiteInfoAsync(PinInfoModel pinInfo)
        {
            var pinSiteInfos = pinInfo?.PinSiteInfos;
            await UpdatePinInfoBeforeAsync(pinInfo);
            await UpdatePinSiteInfoBeforeAsync(pinSiteInfos);

            var pinInfoEntity = pinInfo.MapTo<PinInfo>();
            await _pinInfoRepository.UpdateAsync(pinInfoEntity);

            var pinSiteInfoEntities = pinSiteInfos.MapToList<PinSiteInfo>();
            await _pinSiteInfoRepository.UpdateAsync(pinSiteInfoEntities);
        }

        public async Task DeletePinAndSiteInfoByIdAsync(string pinId)
        {
            if (pinId.IsEmpty())
                return;

            var entities = await _pinInfoRepository.FindByIdsAsync(pinId);
            await ValidateDeleteAsync(entities);

            var pinSiteInfos = await _pinSiteInfoRepository.FindAllAsync(x => x.PinInfoId.Equals(pinId.ToGuid()));

            var pinGroupRelationships = await _pinGroupRelationshipRepositoy.FindAllAsync(x => x.PinInfoId.Equals(pinId.ToGuid()));

            await _pinInfoRepository.RemoveAsync(pinId);
            await _pinSiteInfoRepository.RemoveAsync(pinSiteInfos);
            await _pinGroupRelationshipRepositoy.RemoveAsync(pinGroupRelationships);
        }


        #region 创建前事件
        private async Task CreatePinInfoBeforeAsync(PinInfoModel pinInfo)
        {
            var entity = pinInfo.MapTo<PinInfo>();
            var existPin = await _pinInfoRepository.ExistsAsync(x => x.Id != entity.Id && x.PinOverviewId.Equals(entity.PinOverviewId) && x.PinName.Equals(entity.PinName));

            if (existPin)
            {
                var message = string.Format(L["FieldAlreadyExists"], L["PinName"]);
                throw new ArgumentException(message);
            }

            var pinInfos = await _pinInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(entity.PinOverviewId));

            pinInfo.SortId = pinInfos.Count + 1;
        }

        private async Task CreatePinSiteInfoBeforeAsync(IEnumerable<PinSiteInfoModel> pinSiteInfos)
        {
            if (pinSiteInfos.IsEmpty())
                return;

            var siteGroups = pinSiteInfos?.GroupBy(x => x.SiteInfoId);
            foreach (var sitePins in siteGroups)
            {
                foreach (var channel in sitePins)
                {
                    var entity = channel.MapTo<PinSiteInfo>();
                    var existChannel = await _pinSiteInfoRepository.ExistsAsync(x => x.Id != entity.Id && x.SiteInfoId.Equals(sitePins.Key) && x.ChannelName.Equals(entity.ChannelName));
                    if (existChannel)
                    {
                        throw new ArgumentException(string.Format(L["FieldAlreadyExists"], $"{channel?.SiteName}:{channel?.ChannelName}"));
                    }
                }

            }
        }
        #endregion

        #region 更新前事件
        private async Task UpdatePinInfoBeforeAsync(PinInfoModel pinInfo)
        {
            var entity = pinInfo.MapTo<PinInfo>();
            var existPin = await _pinInfoRepository.ExistsAsync(x => x.Id != entity.Id && x.PinOverviewId.Equals(entity.PinOverviewId) && x.PinName.Equals(entity.PinName));

            if (existPin)
            {
                var message = string.Format(L["FieldAlreadyExists"], L["PinName"]);
                throw new ArgumentException(message);
            }
        }

        private async Task UpdatePinSiteInfoBeforeAsync(IEnumerable<PinSiteInfoModel> pinSiteInfos)
        {
            var siteGroups = pinSiteInfos?.GroupBy(x => x.SiteInfoId);
            foreach (var sitePins in siteGroups)
            {
                foreach (var channel in sitePins)
                {
                    var entity = channel.MapTo<PinSiteInfo>();
                    var existChannel = await _pinSiteInfoRepository.ExistsAsync(x => x.Id != entity.Id && x.SiteInfoId.Equals(sitePins.Key) && x.ChannelName.Equals(entity.ChannelName));
                    if (existChannel)
                    {
                        throw new ArgumentException(string.Format(L["FieldAlreadyExists"], $"{channel?.SiteName}:{channel?.ChannelName}"));
                    }
                }
            }
        }

        #endregion

        #region 删除前事件
        private async Task ValidateDeleteAsync(List<PinInfo> pinInfos)
        {
            foreach (var pinInfo in pinInfos)
            {
                var levels = await _levelRepository?.FindAllAsync(x => x.GroupOrPinId.Equals(pinInfo.Id));
                if (!levels.IsEmpty())
                {
                    var level = levels.FirstOrDefault();
                    throw new Exception(string.Format(L["IsOccupiedBy"], pinInfo.PinName, L["Level"]));
                }

                var timings = await _timingRepository?.FindAllAsync(x => x.GroupOrPinId.Equals(pinInfo.Id));
                if (!timings.IsEmpty())
                {
                    var timing = timings.FirstOrDefault();
                    throw new Exception(string.Format(L["IsOccupiedBy"], pinInfo.PinName, $"{L["TimingName"]}-{timing?.TimingName}"));
                }

                var testItems = await _testItemInfoRepository?.FindAllAsync(x => x.GroupOrPinId.Equals(pinInfo.Id));
                if (!testItems.IsEmpty())
                {
                    var testItem = testItems.FirstOrDefault();
                    throw new Exception(string.Format(L["IsOccupiedBy"], pinInfo.PinName, $"{L["TestItemName"]}-{testItem?.TestItemName}"));
                }
            }
        }
        #endregion
    }
}
