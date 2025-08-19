/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：PinSiteInfoModel.cs
// 功能描述：引脚站点信息
//
// 作者：zhangyingzhong
// 日期：2025/08/14 11:48
// 修改记录(Revision History)
//
//------------------------------------------------------------*/


using KSW.Dtos;

namespace KSW.ATE01.Application.Models.TestPlans
{
    /// <summary>
    /// 引脚站点信息
    /// </summary>
    public class PinSiteInfoModel : DtoBase
    {
        private Guid _siteInfoId;
        private string _siteName;
        private Guid _pinInfoId;
        private string _channelName;
        private int? _sortId;
        private DateTime? _creationTime;
        private DateTime? _lastModificationTime;

        /// <summary>
        /// 站点信息Id
        /// </summary>
        public Guid SiteInfoId
        {
            get => _siteInfoId;
            set => SetProperty(ref _siteInfoId, value);
        }

        /// <summary>
        /// 站点名称
        /// </summary>
        public string SiteName
        {
            get => _siteName;
            set => SetProperty(ref _siteName, value);
        }

        /// <summary>
        /// 引脚信息Id
        /// </summary>
        public Guid PinInfoId
        {
            get => _pinInfoId;
            set => SetProperty(ref _pinInfoId, value);
        }

        /// <summary>
        /// 通道名称
        /// </summary>
        public string ChannelName
        {
            get => _channelName;
            set => SetProperty(ref _channelName, value);
        }

        /// <summary>
        /// 排序Id
        /// </summary>
        public int? SortId
        {
            get => _sortId;
            set => SetProperty(ref _sortId, value);
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreationTime
        {
            get => _creationTime;
            set => SetProperty(ref _creationTime, value);
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? LastModificationTime
        {
            get => _lastModificationTime;
            set => SetProperty(ref _lastModificationTime, value);
        }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 版本号
        ///</summary>
        public byte[] Version { get; set; }
    }
}
