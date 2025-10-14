using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.Dtos;
using KSW.Helpers;
using KSW.Localization;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace KSW.ATE01.Application.Models.TestPlans
{
    public class PinInfoModel : DtoBase, IDataErrorInfo
    {
        private Guid _pinOverviewId;
        private string _pinName;
        private PinType? _pinType;
        private int _sortId;
        private string _groupName;
        private string _channelName;
        private DateTime? _creationTime;
        private DateTime? _lastModificationTime;

        /// <summary>
        /// 引脚总览Id
        /// </summary>
        public Guid PinOverviewId
        {
            get => _pinOverviewId;
            set => SetProperty(ref _pinOverviewId, value);
        }

        /// <summary>
        /// 引脚名称
        /// </summary>
        [Required]
        public string PinName
        {
            get => _pinName;
            set => SetProperty(ref _pinName, value);
        }

        /// <summary>
        /// 引脚类型
        /// </summary>
        [Required]
        public PinType? PinType
        {
            get => _pinType;
            set => SetProperty(ref _pinType, value);
        }

        public string PinTypeDescription => PinType?.Description();

        /// <summary>
        /// 排序ID
        /// </summary>
        public int SortId
        {
            get => _sortId;
            set => SetProperty(ref _sortId, value);
        }

        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName
        {
            get => _groupName;
            set => SetProperty(ref _groupName, value);
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

        /// <summary>
        /// 是否是新增
        /// </summary>
        public bool IsNew { get; set; } = false;

        /// <summary>
        /// 引脚站点信息
        /// </summary>
        public List<PinSiteInfoModel> PinSiteInfos { get; set; }
    }
}
