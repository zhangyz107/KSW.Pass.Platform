using KSW.Domain;
using KSW.Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 测试项电平组
    /// </summary>
    [Description("测试项电平组")]
    public partial class LevelGroup : AggregateRoot<LevelGroup>, IDelete, IVersion
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public LevelGroup() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public LevelGroup(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 项目信息Id
        /// </summary>
        [DisplayName("项目信息Id")]
        [Required]
        public Guid ProjectInfoId { get; set; }

        /// <summary>
        /// 电平组名
        /// </summary>
        [DisplayName("电平组名")]
        [Required]
        public string LevelGroupName { get; set; }

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

        protected override void AddChanges(LevelGroup other)
        {
            AddChange(t => t.ProjectInfoId, other.ProjectInfoId);
            AddChange(t => t.LevelGroupName, other.LevelGroupName);
            AddChange(t => t.CreateTime, other.CreateTime);
            AddChange(t => t.LastModificationTime, other.LastModificationTime);
        }
    }
}
