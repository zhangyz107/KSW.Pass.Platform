using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KSW.ATE01.Application.Models.TestPlans
{
    /// <summary>
    /// 全局参数模型
    /// </summary>
    public class GlobalParameterModel : DtoBase
    {
        private int? _sortId;
        private Guid? _projectInfoId;
        private string _patternFile;
        private string _additionInfo;
        private DateTime? _creationTime;
        private DateTime? _lastModificationTime;

        /// <summary>
        /// 排序
        /// </summary>
        public int? SortId
        {
            get => _sortId;
            set => SetProperty(ref _sortId, value);
        }

        /// <summary>
        /// 项目信息Id
        /// </summary>
        public Guid? ProjectInfoId
        {
            get => _projectInfoId;
            set => SetProperty(ref _projectInfoId, value);
        }

        /// <summary>
        /// 向量文件名称
        /// </summary>
        [Required(ErrorMessage = "TheFieldRequired")]
        public string PatternFile
        {
            get => _patternFile;
            set => SetProperty(ref _patternFile, value);
        }

        /// <summary>
        /// 附加信息
        /// </summary>
        public string AdditionInfo
        {
            get => _additionInfo;
            set => SetProperty(ref _additionInfo, value);
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
