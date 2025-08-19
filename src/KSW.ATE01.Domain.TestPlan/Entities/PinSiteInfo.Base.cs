using KSW.Domain;
using KSW.Domain.Auditing;
using KSW.Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 引脚站点信息
    /// </summary>
    [Description("引脚站点信息")]
    public partial class PinSiteInfo : AggregateRoot<PinSiteInfo>, IDelete, IVersion, IAudited
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public PinSiteInfo() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public PinSiteInfo(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 站点信息Id
        /// </summary>
        [DisplayName("站点信息Id")]
        [Required]
        public Guid SiteInfoId { get; set; }

        /// <summary>
        /// 引脚信息Id
        /// </summary>
        [DisplayName("引脚信息Id")]
        [Required]
        public Guid PinInfoId { get; set; }

        /// <summary>
        /// 通道名称
        /// </summary>
        [DisplayName("通道名称")]
        [Required]
        [MaxLength(100)]
        public string ChannelName { get; set; }

        /// <summary>
        /// 排序Id
        /// </summary>
        [DisplayName("排序Id")]
        public int? SortId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        public DateTime? CreationTime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [DisplayName("最后修改时间")]
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [DisplayName("是否删除")]
        [Required]
        public bool IsDeleted { get; set; }

        protected override void AddChanges(PinSiteInfo other)
        {
            AddChange(t => t.SiteInfoId, other.SiteInfoId);
            AddChange(t => t.PinInfoId, other.PinInfoId);
            AddChange(t => t.ChannelName, other.ChannelName);
            AddChange(t => t.SortId, other.SortId);
            AddChange(t => t.CreationTime, other.CreationTime);
            AddChange(t => t.LastModificationTime, other.LastModificationTime);
        }
    }
}
