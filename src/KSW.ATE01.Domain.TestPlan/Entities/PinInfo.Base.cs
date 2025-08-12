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
    /// 引脚信息
    /// </summary>
    [Description("引脚信息")]
    public partial class PinInfo : AggregateRoot<PinInfo>, IDelete, IVersion
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public PinInfo(): this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public PinInfo(Guid id) : base(id)
        { 
        }

        /// <summary>
        /// 引脚总览Id
        /// </summary>
        [DisplayName("引脚总览Id")]
        [Required]
        public Guid PinOverviewId { get; set; }

        /// <summary>
        /// 引脚名称
        /// </summary>
        [DisplayName("引脚名称")]
        [Required]
        [MaxLength(100)]
        public string PinName { get; set; }

        /// <summary>
        /// 引脚类型
        /// </summary>
        [DisplayName("引脚类型")]
        public int? PinType { get; set; }

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

        protected override void AddChanges(PinInfo other)
        {
            AddChange(t => t.PinOverviewId, other.PinOverviewId);
            AddChange(t => t.PinName, other.PinName);
            AddChange(t => t.PinType, other.PinType);
            AddChange(t => t.CreateTime, other.CreateTime);
            AddChange(t => t.LastModificationTime, other.LastModificationTime);
        }
    }
}
