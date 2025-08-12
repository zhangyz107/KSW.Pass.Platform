using KSW.Domain;
using KSW.Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KSW.ATE01.Domain.TestPlan.Entities
{
    /// <summary>
    /// 测试项电平
    /// </summary>
    [Description("测试项电平")]
    public partial class Level : AggregateRoot<Level>, IDelete, IVersion
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public Level() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public Level(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 测试项电平组Id
        /// </summary>
        [DisplayName("测试项电平组Id")]
        [Required]
        public Guid LevelGroupId { get; set; }

        /// <summary>
        /// 组或引脚Id
        /// </summary>
        [DisplayName("组或引脚Id")]
        public Guid? GroupOrPinId { get; set; }

        /// <summary>
        /// 输入低电压
        /// </summary>
        [DisplayName("输入低电压")]
        public decimal? Vil { get; set; }

        /// <summary>
        /// 输入高电压
        /// </summary>
        [DisplayName("输入高电压")]
        public decimal? Vih { get; set; }

        /// <summary>
        /// 输出低电压
        /// </summary>
        [DisplayName("输出低电压")]
        public decimal? Vol { get; set; }

        /// <summary>
        /// 输出高电压
        /// </summary>
        [DisplayName("输出高电压")]
        public decimal? Voh { get; set; }

        /// <summary>
        /// 低电平输出灌电流
        /// </summary>
        [DisplayName("低电平输出灌电流")]
        public decimal? Iol { get; set; }

        /// <summary>
        /// 高电平输出拉电流
        /// </summary>
        [DisplayName("高电平输出拉电流")]
        public decimal? Ioh { get; set; }

        /// <summary>
        /// 电压基准
        /// </summary>
        [DisplayName("电压基准")]
        public decimal? Vt { get; set; }

        /// <summary>
        /// 错位低电压
        /// </summary>
        [DisplayName("错位低电压")]
        public decimal? Vcl { get; set; }

        /// <summary>
        /// 错位高电压
        /// </summary>
        [DisplayName("错位高电压")]
        public decimal? Vch { get; set; }

        /// <summary>
        /// 供电电压
        /// </summary>
        [DisplayName("供电电压")]
        public decimal? Ps { get; set; }

        /// <summary>
        /// 电流限制
        /// </summary>
        [DisplayName("电流限制")]
        public decimal? I { get; set; }

        /// <summary>
        /// 上电延迟时间
        /// </summary>
        [DisplayName("上电延迟时间")]
        public int? Tdelay { get; set; }

        /// <summary>
        /// 上电顺序
        /// </summary>
        [DisplayName("上电顺序")]
        public int? Sequence { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        [DisplayName("注释")]
        [MaxLength(255)]
        public string? Comment { get; set; }

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

        protected override void AddChanges(Level other)
        {
            AddChange(t => t.LevelGroupId, other.LevelGroupId);
            AddChange(t => t.GroupOrPinId, other.GroupOrPinId);
            AddChange(t => t.Vil, other.Vil);
            AddChange(t => t.Vih, other.Vih);
            AddChange(t => t.Vol, other.Vol);
            AddChange(t => t.Voh, other.Voh);
            AddChange(t => t.Iol, other.Iol);
            AddChange(t => t.Ioh, other.Ioh);
            AddChange(t => t.Vt, other.Vt);
            AddChange(t => t.Vcl, other.Vcl);
            AddChange(t => t.Vch, other.Vch);
            AddChange(t => t.Ps, other.Ps);
            AddChange(t => t.I, other.I);
            AddChange(t => t.Tdelay, other.Tdelay);
            AddChange(t => t.Sequence, other.Sequence);
            AddChange(t => t.Comment, other.Comment);
            AddChange(t => t.CreateTime, other.CreateTime);
            AddChange(t => t.LastModificationTime, other.LastModificationTime);
        }
    }
}
