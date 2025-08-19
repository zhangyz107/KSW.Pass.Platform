using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.Dtos;
using System.Configuration;
using System.Windows.Input;

namespace KSW.ATE01.Application.Models.Projects
{
    /// <summary>
    /// 项目信息模型
    /// </summary>
    public class ProjectInfoModel : DtoBase
    {
        private string _projectName;
        private string _projectPath;
        private string _projectVersion;
        private bool _saveRealTimeText;
        private bool _saveCsv;
        private bool _saveSummary;
        private bool _saveStdf;
        private string _datalogPath;
        private bool _isDoAll;
        private bool _isPrintTime;
        private int _loopCount;
        private int _delayBetweenLoops;
        private int _loopExecuted;
        private int _failCount;
        private bool _stopOnFail;
        private string _releasePath;
        private DateTime? _creationTime;
        private DateTime? _lastModificationTime;
        private ICommand _editCommand;
        private ICommand _developCommand;
        private ICommand _runCommand;

        public ProjectInfoModel()
        {

        }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName
        {
            get => _projectName;
            set => SetProperty(ref _projectName, value);
        }

        /// <summary>
        /// 项目路径
        /// </summary>
        public string ProjectPath
        {
            get => _projectPath;
            set => SetProperty(ref _projectPath, value);
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
        /// 记录RealTime TxT
        /// </summary>
        public bool SaveRealTimeText
        {
            get => _saveRealTimeText;
            set => SetProperty(ref _saveRealTimeText, value);
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
        /// 记录CSV
        /// </summary>
        public bool SaveCsv
        {
            get => _saveCsv;
            set => SetProperty(ref _saveCsv, value);
        }

        /// <summary>
        /// 记录STDF
        /// </summary>
        public bool SaveStdf
        {
            get => _saveStdf;
            set => SetProperty(ref _saveStdf, value);
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
        /// 已执行循环
        /// </summary>
        public int LoopExecuted
        {
            get => _loopExecuted;
            set => SetProperty(ref _loopExecuted, value);
        }

        /// <summary>
        /// 失败数
        /// </summary>
        public int FailCount
        {
            get => _failCount;
            set => SetProperty(ref _failCount, value);
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
        /// 配置文件扩展名
        /// </summary>
        public string ConfigurationExtension => ".atecfg";

        /// <summary>
        /// 执行程序扩展名
        /// </summary>
        public string ExecuteExtension => ".dll";

        /// <summary>
        /// 编辑命令
        /// </summary>
        public ICommand EditCommand
        {
            get => _editCommand;
            set => SetProperty(ref _editCommand, value);
        }

        /// <summary>
        /// 开发命令
        /// </summary>
        public ICommand DelelopCommand
        {
            get => _developCommand;
            set => SetProperty(ref _developCommand, value);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public ICommand RunCommand
        {
            get => _runCommand;
            set => SetProperty(ref _runCommand, value);
        }

    }
}
