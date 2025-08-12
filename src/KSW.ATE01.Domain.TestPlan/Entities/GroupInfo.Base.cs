using KSW.Domain;
using KSW.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 组信息
    /// </summary>
    [Description("组信息")]
    public partial class GroupInfo : AggregateRoot<GroupInfo>, IDelete, IVersion
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public GroupInfo() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public GroupInfo(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 引脚总览Id
        /// </summary>
        [DisplayName("引脚总览Id")]
        [Required]
        public Guid PinOverviewId { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        [DisplayName("组名称")]
        [Required]
        public string GroupName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        public DateTime? CreateTime { get; set; }

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

        protected override void AddChanges(GroupInfo other)
        {
            AddChange(t => t.PinOverviewId, other.PinOverviewId);
            AddChange(t => GroupName, other.GroupName);
            AddChange(t => t.CreateTime, other.CreateTime);
            AddChange(t => t.LastModificationTime, other.LastModificationTime);
        }
    }
}
