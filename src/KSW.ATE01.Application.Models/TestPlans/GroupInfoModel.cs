using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlans
{
    /// <summary>
    /// 引脚分组信息
    /// </summary>
    public class GroupInfoModel : DtoBase
    {
        private Guid _pinOverviewId;
        private string _groupName;
        private DateTime? _creationTime;
        private DateTime? _lastModificationTime;
        private int _sortId;

        /// <summary>
        /// 引脚总览Id
        /// </summary>
        public Guid PinOverviewId
        {
            get => _pinOverviewId;
            set => SetProperty(ref _pinOverviewId, value);
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
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

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
