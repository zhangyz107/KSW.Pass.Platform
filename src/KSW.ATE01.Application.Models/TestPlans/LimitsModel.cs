using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlans
{
    public class LimitsModel : DtoBase
    {
        private int _sortId;
        private Guid? _projectInfoId;
        private int? _testNumber;
        private decimal? _lowLimit;
        private decimal? _highLimit;
        private string _units;
        private string _limitName;
        private int? _failSoftwareBin;
        private int? _passSoftwareBin;
        private int? _failHardwareBin;
        private int? _passHardwareBin;
        private DUTResultType? _dutResult;
        private DateTime? _creationTime;
        private DateTime? _lastModificationTime;

        /// <summary>
        /// 排序Id
        /// </summary>
        public int SortId
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
        /// 测试编号
        /// </summary>
        [Required]
        public int? TestNumber
        {
            get => _testNumber;
            set => SetProperty(ref _testNumber, value);
        }

        /// <summary>
        /// 电压下限
        /// </summary>
        [Required]
        public decimal? LowLimit
        {
            get => _lowLimit;
            set => SetProperty(ref _lowLimit, value);
        }

        /// <summary>
        /// 电压上限
        /// </summary>
        [Required]
        public decimal? HighLimit
        {
            get => _highLimit;
            set => SetProperty(ref _highLimit, value);
        }

        /// <summary>
        /// 单位
        /// </summary>
        [Required]
        public string Units
        {
            get => _units;
            set => SetProperty(ref _units, value);
        }

        /// <summary>
        /// 门限名称
        /// </summary>
        [Required]
        public string LimitName
        {
            get => _limitName;
            set => SetProperty(ref _limitName, value);
        }

        /// <summary>
        /// 软件失效分档
        /// </summary>
        [Required]
        public int? FailSoftwareBin
        {
            get => _failSoftwareBin;
            set => SetProperty(ref _failSoftwareBin, value);
        }

        /// <summary>
        /// 软件成功分档
        /// </summary>
        public int? PassSoftwareBin
        {
            get => _passSoftwareBin;
            set => SetProperty(ref _passSoftwareBin, value);
        }

        /// <summary>
        /// 硬件失效分档
        /// </summary>
        [Required]
        public int? FailHardwareBin
        {
            get => _failHardwareBin;
            set => SetProperty(ref _failHardwareBin, value);
        }

        /// <summary>
        /// 硬件成功分档
        /// </summary>
        public int? PassHardwareBin
        {
            get => _passHardwareBin;
            set => SetProperty(ref _passHardwareBin, value);
        }

        /// <summary>
        /// 被测物结果
        /// </summary>
        [Required]
        public DUTResultType? DutResult
        {
            get => _dutResult;
            set => SetProperty(ref _dutResult, value);
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
    }
}
