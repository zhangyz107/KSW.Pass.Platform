using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlans
{
    /// <summary>
    /// 测试项时钟组
    /// </summary>
    public class TimingGroupModel : DtoBase
    {
        private Guid? _projectInfoId;
        private string _timingGroupName;
        private DateTime? _creationTime;
        private DateTime? _lastModificationTime;

        /// <summary>
        /// 项目信息Id
        /// </summary>
        public Guid? ProjectInfoId
        {
            get => _projectInfoId;
            set => SetProperty(ref _projectInfoId, value);
        }

        /// <summary>
        /// 时钟组名
        /// </summary>
        public string TimingGroupName
        {
            get => _timingGroupName;
            set => SetProperty(ref _timingGroupName, value);
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
        public bool IsNew { get; set; }

    }
}
