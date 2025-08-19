using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.Domain;
using KSW.Domain.Auditing;
using KSW.Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 测试项时钟
    /// </summary>
    [Description("测试项时钟")]
    public partial class Timing : AggregateRoot<Timing>, IDelete, IVersion, IAudited
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public Timing() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public Timing(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 测试项时钟组Id
        /// </summary>
        [DisplayName("测试项时钟组Id")]
        [Required]
        public Guid TimingGroupId { get; set; }

        /// <summary>
        /// 时钟名称
        /// </summary>
        [DisplayName("时钟名称")]
        [Required]
        public string TimingName { get; set; }

        /// <summary>
        /// 周期
        /// </summary>
        [DisplayName("周期")]
        public int? Period { get; set; }

        /// <summary>
        /// 引脚Id
        /// </summary>
        [DisplayName("组或引脚Id")]
        public Guid? GroupOrPinId { get; set; }

        /// <summary>
        /// 波形格式
        /// </summary>
        [DisplayName("波形格式")]
        public int? WaveformFormat { get; set; }

        /// <summary>
        /// 环绕边缘
        /// </summary>
        [DisplayName("环绕边缘")]
        public int? DriveA { get; set; }

        /// <summary>
        /// 起始边缘
        /// </summary>
        [DisplayName("起始边缘")]
        public int? DriveB { get; set; }

        /// <summary>
        /// 返回边缘
        /// </summary>
        [DisplayName("返回边缘")]
        public int? DriveC { get; set; }

        /// <summary>
        /// 关闭边缘
        /// </summary>
        [DisplayName("关闭边缘")]
        public int? DriveD { get; set; }

        /// <summary>
        /// 选通模式
        /// </summary>
        [DisplayName("选通模式")]
        public StrobeModeType? StrobeMode { get; set; }

        /// <summary>
        /// 选通开始时间
        /// </summary>
        [DisplayName("选通开始时间")]
        public int? StrobeA { get; set; }

        /// <summary>
        /// 选通结束时间
        /// </summary>
        [DisplayName("选通结束时间")]
        public int? StrobeB { get; set; }

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

        protected override void AddChanges(Timing other)
        {
            AddChange(t => t.TimingGroupId, other.TimingGroupId);
            AddChange(t => t.TimingName, other.TimingName);
            AddChange(t => t.Period, other.Period);
            AddChange(t => t.GroupOrPinId, other.GroupOrPinId);
            AddChange(t => t.WaveformFormat, other.WaveformFormat);
            AddChange(t => t.DriveA, other.DriveA);
            AddChange(t => t.DriveB, other.DriveB);
            AddChange(t => t.DriveC, other.DriveC);
            AddChange(t => t.DriveD, other.DriveD);
            AddChange(t => t.StrobeMode, other.StrobeMode);
            AddChange(t => t.StrobeA, other.StrobeA);
            AddChange(t => t.StrobeB, other.StrobeB);
            AddChange(t => t.CreationTime, other.CreationTime);
            AddChange(t => t.LastModificationTime, other.LastModificationTime);
        }
    }
}
