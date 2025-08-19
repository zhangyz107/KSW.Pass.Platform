using KSW.Domain;
using KSW.Domain.Auditing;
using KSW.Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 引脚总览
    /// </summary>
    [Description("引脚总览")]
    public partial class PinOverview : AggregateRoot<PinOverview>, IDelete, IVersion, IAudited
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public PinOverview(): this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public PinOverview(Guid id) : base(id)
        { 
        }

        /// <summary>
        /// 项目信息Id
        /// </summary>
        [DisplayName("项目信息Id")]
        [Required]
        public Guid ProjectInfoId { get; set; }

        /// <summary>
        /// 站点数
        /// </summary>
        [DisplayName("站点数")]
        public int? SiteCount { get; set; }

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

        protected override void AddChanges(PinOverview other)
        {
            AddChange(t => t.ProjectInfoId, other.ProjectInfoId);
            AddChange(t => t.SiteCount, other.SiteCount);
            AddChange(t => t.CreationTime, other.CreationTime);
            AddChange(t => t.LastModificationTime, other.LastModificationTime);
        }
    }
}
