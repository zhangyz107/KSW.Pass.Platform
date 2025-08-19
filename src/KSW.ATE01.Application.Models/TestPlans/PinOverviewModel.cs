using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlans
{
    /// <summary>
    /// 引脚概览
    /// </summary>
    public class PinOverviewModel : DtoBase
    {
        private Guid _projectInfoId;
        private int? _siteCount;
        private DateTime? _creationTime;
        private DateTime? _lastModificationTime;

        /// <summary>
        /// 项目信息Id
        /// </summary>
        public Guid ProjectInfoId
        {
            get => _projectInfoId;
            set => SetProperty(ref _projectInfoId, value);
        }

        /// <summary>
        /// 站点数
        /// </summary>
        public int? SiteCount
        {
            get => _siteCount;
            set => SetProperty(ref _siteCount, value);
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
