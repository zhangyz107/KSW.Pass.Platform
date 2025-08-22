
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
using KSW.ATE01.Application.BLLs.Abstractions.Managers;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Data.Repositories.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.ATE01.Project.Base.Models.Errors;
using KSW.Data.EntityFrameworkCore;
using KSW.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace KSW.ATE01.Application.BLLs.Implements.Managers
{
    /// <summary>
    /// 引脚通道管理
    /// </summary>
    public class PinChannelManager : ServiceBase, IPinChannelManager
    {
        #region Fields
        private readonly ISystemUnitOfWork _systemUnitOfWork;
        private readonly IPinInfoRepository _pinInfoRepository;
        private readonly IPinSiteInfoRepository _pinSiteInfoRepository;
        #endregion

        public PinChannelManager(
            IContainerProvider containerProvider,
            ISystemUnitOfWork systemUnitOfWork,
            IPinInfoRepository pinInfoRepository,
            IPinSiteInfoRepository pinSiteInfoRepository) : base(containerProvider)
        {
            _systemUnitOfWork = systemUnitOfWork;
            _pinInfoRepository = pinInfoRepository;
            _pinSiteInfoRepository = pinSiteInfoRepository;
        }

        public async Task CreatePinAndSiteInfoAsync(PinInfoModel pinInfo, IEnumerable<PinSiteInfoModel> pinSiteInfos)
        {
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
            await _systemUnitOfWork.CommitAsync();
        }

        public async Task UpdatePinAndSiteInfoAsync(PinInfoModel pinInfo, IEnumerable<PinSiteInfoModel> pinSiteInfos)
        {
            await UpdatePinInfoBeforeAsync(pinInfo);
            await UpdatePinSiteInfoBeforeAsync(pinSiteInfos);

            var pinInfoEntity = pinInfo.MapTo<PinInfo>();
            await _pinInfoRepository.UpdateAsync(pinInfoEntity);

            var pinSiteInfoEntities = pinSiteInfos.MapToList<PinSiteInfo>();
            await _pinSiteInfoRepository.UpdateAsync(pinSiteInfoEntities);

            await _systemUnitOfWork.CommitAsync();
        }

        public async Task DeletePinAndSiteInfoByIdAsync(string pinId)
        {
            var pinSiteInfos = await _pinSiteInfoRepository.FindAllAsync(x => x.PinInfoId.Equals(pinId.ToGuid()));
            await _pinSiteInfoRepository.RemoveAsync(pinSiteInfos);
            await _pinInfoRepository.RemoveAsync(pinId);
            await _systemUnitOfWork.CommitAsync();
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
        }

        private async Task CreatePinSiteInfoBeforeAsync(IEnumerable<PinSiteInfoModel> pinSiteInfos)
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
    }
}
