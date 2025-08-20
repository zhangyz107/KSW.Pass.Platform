using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.Domain;
using KSW.Domain.Auditing;
using KSW.Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 测试项门限
    /// </summary>
    [Description("测试项门限")]
    public partial class Limits : AggregateRoot<Limits>, IDelete, IVersion, IAudited
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public Limits() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public Limits(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 项目信息Id
        /// </summary>
        [DisplayName("项目信息Id")]
        [Required]
        public Guid ProjectInfoId { get; set; }

        /// <summary>
        /// 测试编号
        /// </summary>
        [DisplayName("测试编号")]
        [Required]
        public int TestNumber { get; set; }

        /// <summary>
        /// 电压下限
        /// </summary>
        [DisplayName("电压下限")]
        public decimal? LowLimit { get; set; }

        /// <summary>
        /// 电压上限
        /// </summary>
        [DisplayName("电压上限")]
        public decimal? HighLimit { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [DisplayName("单位")]
        [MaxLength(50)]
        public string? Units { get; set; }

        /// <summary>
        /// 门限名称
        /// </summary>
        [DisplayName("门限名称")]
        public string? LimitName { get; set; }

        /// <summary>
        /// 软件失效分档
        /// </summary>
        [DisplayName("软件失效分档")]
        public int? FailSoftwareBin { get; set; }

        /// <summary>
        /// 软件成功分档
        /// </summary>
        [DisplayName("软件成功分档")]
        public int? PassSoftwareBin { get; set; }

        /// <summary>
        /// 硬件失效分档
        /// </summary>
        [DisplayName("硬件失效分档")]
        public int? FailHardwareBin { get; set; }

        /// <summary>
        /// 硬件成功分档
        /// </summary>
        [DisplayName("硬件成功分档")]
        public int? PassHardwareBin { get; set; }

        /// <summary>
        /// 被测物结果
        /// </summary>
        [DisplayName("被测物结果")]
        public DUTResultType? DutResult { get; set; }

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

        protected override void AddChanges(Limits other)
        {
            AddChange(t => t.ProjectInfoId, other.ProjectInfoId);
            AddChange(t => t.TestNumber, other.TestNumber);
            AddChange(t => t.LowLimit, other.LowLimit);
            AddChange(t => t.HighLimit, other.HighLimit);
            AddChange(t => t.Units, other.Units);
            AddChange(t => t.LimitName, other.LimitName);
            AddChange(t => t.FailSoftwareBin, other.FailSoftwareBin);
            AddChange(t => t.PassSoftwareBin, other.PassSoftwareBin);
            AddChange(t => t.FailHardwareBin, other.FailHardwareBin);
            AddChange(t => t.PassHardwareBin, other.PassHardwareBin);
            AddChange(t => t.DutResult, other.DutResult);
            AddChange(t => t.CreationTime, other.CreationTime);
            AddChange(t => t.LastModificationTime, other.LastModificationTime);
        }
    }
}
