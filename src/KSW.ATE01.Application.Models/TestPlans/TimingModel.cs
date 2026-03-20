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
    /// <summary>
    /// 测试项时钟模型
    /// </summary>
    public class TimingModel : DtoBase
    {
        private int _sortId;
        private Guid? _timingGroupId;
        private string _timingName;
        private decimal? _period;
        private Guid? _groupOrPinId;
        private TimingformatType? _waveformFormat;
        private decimal? _driveA;
        private decimal? _driveB;
        private decimal? _driveC;
        private decimal? _driveD;
        private StrobeModeType? _strobeMode;
        private decimal? _strobeA;
        private decimal? _strobeB;
        private string _comment;
        private DateTime? _creationTime;
        private DateTime? _lastModificationTime;

        /// <summary>
        /// 排序
        /// </summary>
        public int SortId
        {
            get => _sortId;
            set => SetProperty(ref _sortId, value);
        }


        /// <summary>
        /// 测试项时钟组Id
        /// </summary>
        public Guid? TimingGroupId
        {
            get => _timingGroupId;
            set => SetProperty(ref _timingGroupId, value);
        }

        /// <summary>
        /// 时钟组名称
        /// </summary>
        public string TimingGroupName { get; set; }

        /// <summary>
        /// 时钟名称
        /// </summary>
        [Required(ErrorMessage = "TheFieldRequired")]
        public string TimingName
        {
            get => _timingName;
            set => SetProperty(ref _timingName, value);
        }

        /// <summary>
        /// 周期
        /// </summary>
        [Required(ErrorMessage = "TheFieldRequired")]
        public decimal? Period
        {
            get => _period;
            set => SetProperty(ref _period, value);
        }

        /// <summary>
        /// 组或引脚Id
        /// </summary>
        [Required(ErrorMessage = "TheFieldRequired")]
        public Guid? GroupOrPinId
        {
            get => _groupOrPinId;
            set
            {
                if (value != null)
                    SetProperty(ref _groupOrPinId, value);
            }
        }

        /// <summary>
        /// 引脚或组名称
        /// </summary>
        public string PinOrGroupName { get; set; }

        /// <summary>
        /// 波形格式
        /// </summary>
        [Required(ErrorMessage = "TheFieldRequired")]
        public TimingformatType? WaveformFormat
        {
            get => _waveformFormat;
            set => SetProperty(ref _waveformFormat, value);
        }

        /// <summary>
        /// 波形格式描述
        /// </summary>
        public string WaveformFormatDescription => _waveformFormat?.Description();

        /// <summary>
        /// 环绕边缘
        /// </summary>
        [Required(ErrorMessage = "TheFieldRequired")]
        public decimal? DriveA
        {
            get => _driveA;
            set => SetProperty(ref _driveA, value);
        }

        /// <summary>
        /// 起始边缘
        /// </summary>
        [Required(ErrorMessage = "TheFieldRequired")]
        public decimal? DriveB
        {
            get => _driveB;
            set => SetProperty(ref _driveB, value);
        }

        /// <summary>
        /// 返回边缘
        /// </summary>
        [Required(ErrorMessage = "TheFieldRequired")]
        public decimal? DriveC
        {
            get => _driveC;
            set => SetProperty(ref _driveC, value);
        }

        /// <summary>
        /// 关闭边缘
        /// </summary>
        [Required(ErrorMessage = "TheFieldRequired")]
        public decimal? DriveD
        {
            get => _driveD;
            set => SetProperty(ref _driveD, value);
        }

        /// <summary>
        /// 选通模式
        /// </summary>
        public StrobeModeType? StrobeMode
        {
            get => _strobeMode;
            set => SetProperty(ref _strobeMode, value);
        }

        /// <summary>
        /// 选通模式描述
        /// </summary>
        public string StrobeModeDescription => _strobeMode?.Description();

        /// <summary>
        /// 选通开始时间
        /// </summary>
        public decimal? StrobeA
        {
            get => _strobeA;
            set => SetProperty(ref _strobeA, value);
        }

        /// <summary>
        /// 选通结束时间
        /// </summary>
        public decimal? StrobeB
        {
            get => _strobeB;
            set => SetProperty(ref _strobeB, value);
        }

        /// <summary>
        /// 注释
        /// </summary>
        public string? Comment
        {
            get => _comment;
            set => SetProperty(ref _comment, value);
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
