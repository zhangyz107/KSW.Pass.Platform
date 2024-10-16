using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.TestPlan
{
    /// <summary>
    /// 电压限制模型
    /// </summary>
    public class LimitsModel : DtoBase
    {
        private Guid _testItemId;
        private string _limitName;
        private int _testNumber;
        private decimal _lowLimit;
        private decimal _highLimit;
        private string _units;
        private int _failSoftwareBin;
        private int _passSoftwareBin;
        private int _failHardwareBin;
        private int _passHardwareBin;
        private DUTResultType _dUTResult;

        /// <summary>
        /// 测试项Id
        /// </summary>
        public Guid TestItemId
        {
            get => _testItemId;
            set => SetProperty(ref _testItemId, value);
        }

        /// <summary>
        /// 电压限制名称
        /// </summary>
        public string LimitName
        {
            get => _limitName;
            set => SetProperty(ref _limitName, value);
        }

        /// <summary>
        /// 测试编号
        /// </summary>
        public int TestNumber 
        {
            get => _testNumber;
            set => SetProperty(ref _testNumber, value);
        }

        /// <summary>
        /// 电压下限
        /// </summary>
        public decimal LowLimit 
        { 
            get => _lowLimit; 
            set => SetProperty(ref _lowLimit, value);
        }

        /// <summary>
        /// 电压上限
        /// </summary>
        public decimal HighLimit 
        {
            get => _highLimit; 
            set => SetProperty(ref _highLimit, value);
        }

        /// <summary>
        /// 单位
        /// </summary>
        public string Units 
        { 
            get => _units; 
            set => SetProperty(ref _units, value); 
        }

        /// <summary>
        /// 软件Bin号
        /// </summary>
        public int FailSoftwareBin 
        {
            get => _failSoftwareBin; 
            set => SetProperty(ref _failSoftwareBin, value);
        }

        /// <summary>
        /// 软件Bin号
        /// </summary>
        public int PassSoftwareBin 
        {
            get => _passSoftwareBin; 
            set => SetProperty(ref _passSoftwareBin, value);
        }

        /// <summary>
        /// 硬件Bin号
        /// </summary>
        public int FailHardwareBin 
        { 
            get => _failHardwareBin; 
            set => SetProperty(ref _failHardwareBin, value); 
        }

        /// <summary>
        /// 硬件Bin号
        /// </summary>
        public int PassHardwareBin 
        { 
            get => _passHardwareBin;
            set => SetProperty(ref _passHardwareBin, value); 
        }

        /// <summary>
        /// 测试结果
        /// </summary>
        public DUTResultType DUTResult 
        { 
            get => _dUTResult; 
            set => SetProperty(ref _dUTResult, value);
        }
    }
}
