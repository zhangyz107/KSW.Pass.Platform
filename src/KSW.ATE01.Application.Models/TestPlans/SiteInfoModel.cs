using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlans
{
    /// <summary>
    /// 站点头模型
    /// </summary>
    public class SiteInfoModel : DtoBase
    {
        private Guid _pinOverviewId;
        private int _sortId;
        private string _siteName;
        private DateTime? _createTime;
        private DateTime? _lastModificationTime;
        private bool _isSelected;

        /// <summary>
        /// 引脚总览Id
        /// </summary>
        public Guid PinOverviewId
        {
            get => _pinOverviewId;
            set => SetProperty(ref _pinOverviewId, value);
        }

        /// <summary>
        /// 序号
        /// </summary>
        public int SortId
        {
            get => _sortId;
            set => SetProperty(ref _sortId, value);
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
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime
        {
            get => _createTime;
            set => SetProperty(ref _createTime, value);
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
