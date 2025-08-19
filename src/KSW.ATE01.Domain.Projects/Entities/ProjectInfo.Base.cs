using KSW.Domain;
using KSW.Domain.Auditing;
using KSW.Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KSW.ATE01.Domain.Projects.Entities
{
    [Description("项目信息")]
    public partial class ProjectInfo : AggregateRoot<ProjectInfo>, IDelete, IVersion, IAudited
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public ProjectInfo() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public ProjectInfo(Guid id) : base(id)
        { 
        }

        /// <summary>
        /// 项目名称
        /// </summary>
        [DisplayName("项目名称")]
        [Required]
        [MaxLength(100)]
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目路径
        /// </summary>
        [DisplayName("项目路径")]
        [Required]
        [MaxLength(200)]
        public string ProjectPath { get; set; }

        /// <summary>
        /// 项目版本
        /// </summary>
        [DisplayName("项目版本")]
        [MaxLength(200)]
        public string? ProjectVersion { get; set; }

        /// <summary>
        /// 记录RealTimeTxT
        /// </summary>
        [DisplayName("记录RealTimeTxT")]
        public bool? SaveRealTimeText { get; set; }

        /// <summary>
        /// 记录CSV
        /// </summary>
        [DisplayName("记录CSV")]
        public bool? SaveCsv { get; set; }

        /// <summary>
        /// 记录Summary
        /// </summary>
        [DisplayName("记录Summary")]
        public bool? SaveSummary { get; set; }

        /// <summary>
        /// 记录STDF
        /// </summary>
        [DisplayName("记录STDF")]
        public bool? SaveStdf { get; set; }

        /// <summary>
        /// 日志路径
        /// </summary>
        [DisplayName("日志路径")]
        [MaxLength(255)]
        public string? DatalogPath { get; set; }

        /// <summary>
        /// 是否DoAll
        /// </summary>
        [DisplayName("是否DoAll")]
        public bool? IsDoAll { get; set; }

        /// <summary>
        /// 是否打印时间
        /// </summary>
        [DisplayName("是否打印时间")]
        public bool? IsPrintTime { get; set; }

        /// <summary>
        /// 循环次数
        /// </summary>
        [DisplayName("循环次数")]
        public int? LoopCount { get; set; }

        /// <summary>
        /// 循环间时延
        /// </summary>
        [DisplayName("循环间时延")]
        public int? DelayBetweenLoops { get; set; }

        /// <summary>
        /// 失败时停止
        /// </summary>
        [DisplayName("失败时停止")]
        public bool? StopOnFail { get; set; }

        /// <summary>
        /// 发布路径
        /// </summary>
        [DisplayName("发布路径")]
        [MaxLength(255)]
        public string? ReleasePath { get; set; }

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

        protected override void AddChanges(ProjectInfo other)
        {
            AddChange(t => ProjectName, other.ProjectName);
            AddChange(t => ProjectPath, other.ProjectPath);
            AddChange(t => ProjectVersion, other.ProjectVersion);
            AddChange(t => SaveRealTimeText, other.SaveRealTimeText);
            AddChange(t => SaveCsv, other.SaveCsv);
            AddChange(t => SaveSummary, other.SaveSummary);
            AddChange(t => SaveStdf, other.SaveStdf);
            AddChange(t => DatalogPath, other.DatalogPath);
            AddChange(t => IsDoAll, other.IsDoAll);
            AddChange(t => IsPrintTime, other.IsPrintTime);
            AddChange(t => LoopCount, other.LoopCount);
            AddChange(t => DelayBetweenLoops, other.DelayBetweenLoops);
            AddChange(t => StopOnFail, other.StopOnFail);
            AddChange(t => ReleasePath, other.ReleasePath);
            AddChange(t => CreationTime, other.CreationTime);
            AddChange(t => LastModificationTime, other.LastModificationTime);
        }
    }
}
