using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.Dtos;

namespace KSW.ATE01.Application.Models.Projects
{
    /// <summary>
    /// 项目信息模型
    /// </summary>
    public class ProjectInfoModel : DtoBase
    {
        private string _projectName;
        private TestPlanType _testPlanType;
        private string _projectPath;
        private DateTime _createTime;
        private string _projectVersion;
        private bool _saveRealTimeText;
        private bool _saveCsv;
        private bool _saveSummary;
        private bool _saveSTDF;
        private string _datalogPath;
        private bool _isDoAll;
        private bool _isPrintTime;
        private bool _isOffLine;
        private int _loopCount;
        private int _delayBetweenLoops;
        private bool _stopOnFail;
        private string _releasePath;

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName
        {
            get => _projectName;
            set => SetProperty(ref _projectName, value);
        }

        /// <summary>
        /// 测试计划类型
        /// </summary>
        public TestPlanType TestPlanType
        {
            get => _testPlanType;
            set => SetProperty(ref _testPlanType, value);
        }

        public string TestPlanTypeDescription => TestPlanType.Description();

        /// <summary>
        /// 项目路径
        /// </summary>
        public string ProjectPath
        {
            get => _projectPath;
            set => SetProperty(ref _projectPath, value);
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get => _createTime;
            set => SetProperty(ref _createTime, value);
        }

        /// <summary>
        /// 项目版本
        /// </summary>
        public string ProjectVersion
        {
            get => _projectVersion;
            set => SetProperty(ref _projectVersion, value);
        }

        /// <summary>
        /// 测试计划扩展名
        /// </summary>
        public string TestPlanExtension => _testPlanType == TestPlanType.Excel ? ".xlsx" : ".csv";

        /// <summary>
        /// 配置文件扩展名
        /// </summary>
        public string ConfigurationExtension => ".atecfg";

        /// <summary>
        /// 执行程序扩展名
        /// </summary>
        public string ExecuteExtension => ".dll";

        /// <summary>
        /// 记录RealTime TxT
        /// </summary>
        public bool SaveRealTimeText
        {
            get => _saveRealTimeText;
            set => SetProperty(ref _saveRealTimeText, value);
        }

        /// <summary>
        /// 记录CSV
        /// </summary>
        public bool SaveCsv
        {
            get => _saveCsv;
            set => SetProperty(ref _saveCsv, value);
        }

        /// <summary>
        /// 记录Summary
        /// </summary>
        public bool SaveSummary
        {
            get => _saveSummary;
            set => SetProperty(ref _saveSummary, value);
        }

        /// <summary>
        /// 记录STDF
        /// </summary>
        public bool SaveSTDF
        {
            get => _saveSTDF;
            set => SetProperty(ref _saveSTDF, value);
        }

        /// <summary>
        /// 日志路径
        /// </summary>
        public string DatalogPath
        {
            get => _datalogPath;
            set => SetProperty(ref _datalogPath, value);
        }

        /// <summary>
        /// 是否DoAll
        /// </summary>
        public bool IsDoAll
        {
            get => _isDoAll;
            set => SetProperty(ref _isDoAll, value);
        }

        /// <summary>
        /// 是否打印时间
        /// </summary>
        public bool IsPrintTime
        {
            get => _isPrintTime;
            set => SetProperty(ref _isPrintTime, value);
        }

        /// <summary>
        /// 是否离线模式
        /// </summary>
        public bool IsOffLine
        {
            get => _isOffLine;
            set => SetProperty(ref _isOffLine, value);
        }

        /// <summary>
        /// 循环次数
        /// </summary>
        public int LoopCount
        {
            get => _loopCount;
            set
            {
                if (value != _loopCount)
                {
                    if (value < 0)
                    {
                        value = 0;
                    }
                    SetProperty(ref _loopCount, value);
                }
            }
        }

        /// <summary>
        /// 循环间时延
        /// </summary>
        public int DelayBetweenLoops
        {
            get => _delayBetweenLoops;
            set
            {
                if (value != _delayBetweenLoops)
                {
                    if (value > 300)
                    {
                        value = 300;
                    }
                    else if (value < 0)
                    {
                        value = 0;
                    }
                    SetProperty(ref _delayBetweenLoops, value);
                }
            }
        }

        /// <summary>
        /// 失败时停止
        /// </summary>
        public bool StopOnFail
        {
            get => _stopOnFail;
            set => SetProperty(ref _stopOnFail, value);
        }

        /// <summary>
        /// 发布路径
        /// </summary>
        public string ReleasePath 
        {
            get => _releasePath; 
            set => SetProperty(ref _releasePath, value);
        }
    }
}
