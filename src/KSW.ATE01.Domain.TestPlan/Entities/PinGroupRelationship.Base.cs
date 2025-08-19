using KSW.Domain;
using KSW.Domain.Auditing;
using KSW.Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 引脚与组关系
    /// </summary>
    [Description("引脚与组关系")]
    public partial class PinGroupRelationship : AggregateRoot<PinGroupRelationship>, IVersion, IAudited
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public PinGroupRelationship() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public PinGroupRelationship(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 引脚信息Id
        /// </summary>
        [DisplayName("引脚信息Id")]
        [Required]
        public Guid PinInfoId { get; set; }

        /// <summary>
        /// 组信息Id
        /// </summary>
        [DisplayName("组信息Id")]
        [Required]
        public Guid GroupInfoId { get; set; }

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


        protected override void AddChanges(PinGroupRelationship other)
        {
            AddChange(t => t.PinInfoId, other.PinInfoId);
            AddChange(t => t.GroupInfoId, other.GroupInfoId);
            AddChange(t => t.CreationTime, other.CreationTime);
            AddChange(t => t.LastModificationTime, other.LastModificationTime);
        }
    }
}
