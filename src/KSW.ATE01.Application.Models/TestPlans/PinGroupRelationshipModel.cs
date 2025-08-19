using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlans
{
    /// <summary>
    /// 引脚与组关系
    /// </summary>
    public class PinGroupRelationshipModel : DtoBase
    {
        private Guid? _pinInfoId;
        private string _pinName;
        private Guid? _groupInfoId;
        private string _groupName;
        private DateTime? _creationTime;
        private DateTime? _lastModificationTime;
        private int _sortId;

        /// <summary>
        /// 引脚信息Id
        /// </summary>
        public Guid? PinInfoId
        {
            get => _pinInfoId;
            set => SetProperty(ref _pinInfoId, value);
        }

        /// <summary>
        /// 引脚名称
        /// </summary>
        public string PinName
        {
            get => _pinName;
            set => SetProperty(ref _pinName, value);
        }

        /// <summary>
        /// 组信息Id
        /// </summary>
        public Guid? GroupInfoId
        {
            get => _groupInfoId;
            set => SetProperty(ref _groupInfoId, value);
        }

        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName
        {
            get => _groupName;
            set => SetProperty(ref _groupName, value);
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreationTime
        {
            get => _creationTime;
            set => SetProperty(ref _creationTime, value);
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? LastModificationTime
        {
            get => _lastModificationTime;
            set => SetProperty(ref _lastModificationTime, value);
        }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortId
        {
            get => _sortId;
            set => SetProperty(ref _sortId, value);
        }

        /// <summary>
        /// 版本号
        ///</summary>
        public byte[] Version { get; set; }

        /// <summary>
        /// 是否是新增
        /// </summary>
        public bool IsNew { get; set; } = false;
    }
}
