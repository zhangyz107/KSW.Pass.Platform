using KSW.Domain;
using KSW.Domain.Auditing;
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
    /// 全局参数
    /// </summary>
    [Description("全局参数")]
    public partial class GlobalParameter : AggregateRoot<GlobalParameter>, IDelete, IVersion, IAudited
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public GlobalParameter() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public GlobalParameter(Guid id) : base(id)
        {
        }

        /// <summary>
        /// 项目信息Id
        /// </summary>
        [DisplayName("项目信息Id")]
        [Required]
        public Guid ProjectInfoId { get; set; }

        /// <summary>
        /// 向量文件名称
        /// </summary>
        [DisplayName("向量文件名称")]
        [Required]
        public string PatternFile { get; set; }

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

        protected override void AddChanges(GlobalParameter other)
        {
            AddChange(t => t.ProjectInfoId, other.ProjectInfoId);
            AddChange(t => t.PatternFile, other.PatternFile);
            AddChange(t => t.AdditionInfo, other.AdditionInfo);
            AddChange(t => t.CreationTime, other.CreationTime);
            AddChange(t => t.LastModificationTime, other.LastModificationTime);
        }
    }
}
