using KSW.Domain;
using KSW.Domain.Auditing;
using KSW.Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 测试项信息
    /// </summary>
    [Description("测试项信息")]
    public partial class TestItemInfo : AggregateRoot<TestItemInfo>, IDelete, IVersion, IAudited
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public TestItemInfo() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public TestItemInfo(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 项目信息Id
        /// </summary>
        [DisplayName("项目信息Id")]
        [Required]
        public Guid ProjectInfoId { get; set; }

        /// <summary>
        /// 测试项名称
        /// </summary>
        [DisplayName("测试项名称")]
        [Required]
        [MaxLength(100)]
        public string TestItemName { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        [DisplayName("方法名称")]
        [Required]
        [MaxLength(100)]
        public string FunctionName { get; set; }

        /// <summary>
        /// 激励
        /// </summary>
        [DisplayName("激励")]
        [Required]
        public decimal Force { get; set; }

        /// <summary>
        /// 组或引脚Id
        /// </summary>
        [DisplayName("组或引脚Id")]
        public Guid? GroupOrPinId { get; set; }

        /// <summary>
        /// 测试项门限Id
        /// </summary>
        [DisplayName("测试项门限Id")]
        [Required]
        public Guid LimitsId { get; set; }

        /// <summary>
        /// 测试项电平组Id
        /// </summary>
        [DisplayName("测试项电平组Id")]
        public Guid? LevelGroupId { get; set; }

        /// <summary>
        /// 测试项时钟组Id
        /// </summary>
        [DisplayName("测试项时钟组Id")]
        public Guid? TimingGroupId { get; set; }

        /// <summary>
        /// 附加信息
        /// </summary>
        [DisplayName("附加信息")]
        public string? AdditionInfo { get; set; }

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

        protected override void AddChanges(TestItemInfo other)
        {
            AddChange(t => t.ProjectInfoId, other.ProjectInfoId);
            AddChange(t => t.TestItemName, other.TestItemName);
            AddChange(t => t.FunctionName, other.FunctionName);
            AddChange(t => t.Force, other.Force);
            AddChange(t => t.GroupOrPinId, other.GroupOrPinId);
            AddChange(t => t.LimitsId, other.LimitsId);
            AddChange(t => t.LevelGroupId, other.LevelGroupId);
            AddChange(t => t.TimingGroupId, other.TimingGroupId);
            AddChange(t => t.AdditionInfo, other.AdditionInfo);
            AddChange(t => t.CreationTime, other.CreationTime);
            AddChange(t => t.LastModificationTime, other.LastModificationTime);
        }
    }
}
