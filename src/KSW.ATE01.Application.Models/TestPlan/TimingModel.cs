using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.Dtos;

namespace KSW.ATE01.Application.Models.TestPlan
{
    /// <summary>
    /// 时钟模型
    /// </summary>
    public class TimingModel : DtoBase
    {
        private string _timingName;
        private int _period;
        private Guid _pinId;
        private string _pinSetup;
        private string _fmt;
        private string _driveA;
        private string _driveB;
        private string _driveC;
        private string _driveD;
        private StrobeModeType _strobeMode;
        private int _strobeA;
        private int _strobeB;

        /// <summary>
        /// 时钟名称
        /// </summary>
        public string TimingName
        {
            get => _timingName;
            set => SetProperty(ref _timingName, value);
        }

        /// <summary>
        /// 周期
        /// </summary>
        public int Period
        {
            get => _period;
            set => SetProperty(ref _period, value);
        }

        /// <summary>
        /// 引脚Id
        /// </summary>
        public Guid PinId
        {
            get => _pinId;
            set => SetProperty(ref _pinId, value);
        }

        /// <summary>
        /// 限制名称
        /// </summary>
        public string PinSetup
        {
            get => _pinSetup;
            set => SetProperty(ref _pinSetup, value);
        }

        /// <summary>
        /// 波形格式
        /// </summary>
        public string Fmt
        {
            get => _fmt;
            set => SetProperty(ref _fmt, value);
        }

        /// <summary>
        /// DriveA
        /// </summary>
        public string DriveA 
        { 
            get => _driveA; 
            set => SetProperty(ref _driveA, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public string DriveB 
        { 
            get => _driveB; 
            set => SetProperty(ref _driveB, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public string DriveC 
        { 
            get => _driveC; 
            set => SetProperty(ref _driveC, value); 
        }

        /// <summary>
        /// 
        /// </summary>
        public string DriveD 
        {
            get => _driveD;
            set => SetProperty(ref _driveD, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public StrobeModeType StrobeMode
        { 
            get => _strobeMode; 
            set => SetProperty(ref _strobeMode, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public int StrobeA 
        { 
            get => _strobeA;
            set => SetProperty(ref _strobeA, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public int StrobeB 
        { 
            get => _strobeB; 
            set => SetProperty(ref _strobeB, value);
        }
    }
}
