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
    /// 测试项时钟组
    /// </summary>
    [Description("测试项时钟组")]
    public partial class TimingGroup : AggregateRoot<TimingGroup>, IDelete, IVersion
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public TimingGroup() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public TimingGroup(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 项目信息Id
        /// </summary>
        [DisplayName("项目信息Id")]
        [Required]
        public Guid ProjectInfoId { get; set; }

        /// <summary>
        /// 时钟组名
        /// </summary>
        [DisplayName("时钟组名")]
        [Required]
        public string TimingGroupName { get; set; }

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

        protected override void AddChanges(TimingGroup other)
        {
            AddChange(t => t.ProjectInfoId, other.ProjectInfoId);
            AddChange(t => t.TimingGroupName, other.TimingGroupName);
            AddChange(t => t.CreateTime, other.CreateTime);
            AddChange(t => t.LastModificationTime, other.LastModificationTime);
        }
    }
}
