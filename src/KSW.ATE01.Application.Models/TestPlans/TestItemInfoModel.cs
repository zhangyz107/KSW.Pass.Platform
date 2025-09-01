using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KSW.ATE01.Application.Models.TestPlans
{
    /// <summary>
    /// 测试项信息模型
    /// </summary>
    public class TestItemInfoModel : DtoBase
    {
        private int? _sortId;
        private Guid? _projectInfoId;
        private string _testItemName;
        private string _functionName;
        private decimal? _force;
        private Guid? _groupOrPinId;
        private Guid? _limitsId;
        private Guid? _levelGroupId;
        private string _levelGroupName;
        private Guid? _timingGroupId;
        private string _timingGroupName;
        private string _additionInfo;
        private int? _flowIndex;
        private bool _enable;
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
        /// 测试项名称
        /// </summary>
        public string TestItemName
        {
            get => _testItemName;
            set => SetProperty(ref _testItemName, value);
        }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string FunctionName
        {
            get => _functionName;
            set => SetProperty(ref _functionName, value);
        }

        /// <summary>
        /// 激励
        /// </summary>
        public decimal? Force
        {
            get => _force;
            set => SetProperty(ref _force, value);
        }

        /// <summary>
        /// 组或引脚Id
        /// </summary>
        public Guid? GroupOrPinId
        {
            get => _groupOrPinId;
            set => SetProperty(ref _groupOrPinId, value);
        }

        /// <summary>
        /// 引脚或组名称
        /// </summary>
        public string PinOrGroupName { get; set; }

        /// <summary>
        /// 测试项门限Id
        /// </summary>
        public Guid? LimitsId
        {
            get => _limitsId;
            set => SetProperty(ref _limitsId, value);
        }

        /// <summary>
        /// 门限名称
        /// </summary>
        public string LimitName { get; set; }

        /// <summary>
        /// 测试项电平组Id
        /// </summary>
        public Guid? LevelGroupId
        {
            get => _levelGroupId;
            set => SetProperty(ref _levelGroupId, value);
        }

        /// <summary>
        /// 电平组名
        /// </summary>
        public string LevelGroupName
        {
            get => _levelGroupName;
            set => SetProperty(ref _levelGroupName, value);
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
        /// 时钟组名
        /// </summary>
        public string TimingGroupName
        {
            get => _timingGroupName;
            set => SetProperty(ref _timingGroupName, value);
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
        /// 流程序号
        /// </summary>
        public int? FlowIndex
        {
            get => _flowIndex;
            set => SetProperty(ref _flowIndex, value);
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable
        {
            get => _enable;
            set => SetProperty(ref _enable, value);
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
        /// 向上命令
        /// </summary>
        public ICommand ArrowUpCommand { get; set; }

        /// <summary>
        /// 向下命令
        /// </summary>
        public ICommand ArrowDownCommand { get; set; }
    }
}
