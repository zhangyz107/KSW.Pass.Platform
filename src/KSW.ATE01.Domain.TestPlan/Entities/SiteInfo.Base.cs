using KSW.Domain;
using KSW.Domain.Auditing;
using KSW.Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 站点信息
    /// </summary>
    [Description("站点信息")]
    public partial class SiteInfo : AggregateRoot<SiteInfo>, IDelete, IVersion, IAudited
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public SiteInfo() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public SiteInfo(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 引脚总览Id
        /// </summary>
        [DisplayName("引脚总览Id")]
        [Required]
        public Guid PinOverviewId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DisplayName("序号")]
        [Required]
        public int SortId { get; set; }

        /// <summary>
        /// 站点名称
        /// </summary>
        [DisplayName("站点名称")]
        [Required]
        [MaxLength(100)]
        public string SiteName { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        [DisplayName("是否选中")]
        public bool? IsSelected { get; set; }

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

        protected override void AddChanges(SiteInfo other)
        {
            AddChange(t => t.PinOverviewId, other.PinOverviewId);
            AddChange(t => t.SiteName, other.SiteName);
            AddChange(t => t.SortId, other.SortId);
            AddChange(t => t.IsSelected, other.IsSelected);
            AddChange(t => t.CreationTime, other.CreationTime);
            AddChange(t => t.LastModificationTime, other.LastModificationTime);
        }
    }
}
