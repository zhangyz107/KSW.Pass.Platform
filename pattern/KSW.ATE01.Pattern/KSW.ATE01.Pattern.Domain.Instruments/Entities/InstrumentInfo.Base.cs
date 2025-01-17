using KSW.ATE01.Pattern.Domain.Instruments.Core.Enums;
using KSW.Domain;
using KSW.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Domain.Instruments.Entities
{
    /// <summary>
    /// 配置
    /// </summary>
    [Description("设备")]
    public partial class InstrumentInfo : AggregateRoot<InstrumentInfo>, IDelete
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public InstrumentInfo() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="id"></param>
        public InstrumentInfo(Guid id) : base(id)
        {

        }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string InstrumentName { get; set; }

        /// <summary>
        /// Ip地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 本机端口号
        /// </summary>
        public int LocalPort { get; set; }

        /// <summary>
        /// 连接类型
        /// </summary>
        public IOTypeEnum ConnectType { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int SortId { get; set; }

        /// <summary>
        /// 删除状态
        ///</summary>
        [DisplayName("删除状态")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 添加变更列表
        /// </summary>
        protected override void AddChanges(InstrumentInfo other)
        {
            AddChange(t => t.InstrumentName, other.InstrumentName);
            AddChange(t => t.IpAddress, other.IpAddress);
            AddChange(t => t.Port, other.Port);
            AddChange(t => t.LocalPort, other.LocalPort);
            AddChange(t => t.SortId, other.SortId);
            AddChange(t => t.ConnectType, other.ConnectType);
        }
    }
}
