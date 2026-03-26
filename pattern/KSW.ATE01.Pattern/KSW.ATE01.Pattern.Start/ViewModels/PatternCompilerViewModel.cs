/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：PatternCompilerViewModel.cs
// 功能描述：向量编译视图模型
//
// 作者：zhangyingzhong
// 日期：2024/1/10 15:57
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Patterns;
using KSW.ATE01.Pattern.Application.Events;
using KSW.ATE01.Pattern.Application.Events.Patterns;
using KSW.Ui;
using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Start.ViewModels
{
    /// <summary>
    /// 向量编译视图模型
    /// </summary>
    public class PatternCompilerViewModel : ViewModelBase
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IPatternCompilerBLL _patternCompilerBLL;
        private string _patternFilePath;
        private bool _patternFileIsDir;
        private string[] _patternFiles;

        private string _testPlanName;
        private bool _testPlanIsDir;
        private string[] _testPlanFiles;

        private string _outputDir;
        #endregion

        #region Properties

        public string PatternFilePath
        {
            get => _patternFilePath;
            set => SetProperty(ref _patternFilePath, value);
        }

        public bool PatternFileIsDir
        {
            get => _patternFileIsDir;
            set
            {
                if (SetProperty(ref _patternFileIsDir, value))
                    ClearPatternFile();
            }
        }

        public string TestPlanName
        {
            get => _testPlanName;
            set => SetProperty(ref _testPlanName, value);
        }

        public bool TestPlanIsDir
        {
            get => _testPlanIsDir;
            set
            {
                if (SetProperty(ref _testPlanIsDir, value))
                    ClearTestPlan();
            }
        }

        public string OutputDir
        {
            get => _outputDir;
            set => SetProperty(ref _outputDir, value);
        }
        #endregion

        #region Command
        private DelegateCommand _patternFileBrowseCommand;
        public DelegateCommand PatternFileBrowseCommand =>
            _patternFileBrowseCommand ?? (_patternFileBrowseCommand = new DelegateCommand(ExecutePatternFileBrowseCommand));

        private DelegateCommand _testPlanBrowseCommand;
        public DelegateCommand TestPlanBrowseCommand =>
            _testPlanBrowseCommand ?? (_testPlanBrowseCommand = new DelegateCommand(ExecuteTestPlanBrowseCommand));

        private DelegateCommand _outputBrowseCommand;
        public DelegateCommand OutputBrowseCommand =>
            _outputBrowseCommand ?? (_outputBrowseCommand = new DelegateCommand(ExecuteOutputBrowseCommand));

        private AsyncDelegateCommand _compilerCommand;
        public AsyncDelegateCommand CompilerCommand =>
            _compilerCommand ?? (_compilerCommand = new AsyncDelegateCommand(ExecuteCompilerCommand));

        private DelegateCommand _analyzeAndCompileCommand;
        public DelegateCommand AnalyzeAndCompileCommand =>
            _analyzeAndCompileCommand ?? (_analyzeAndCompileCommand = new DelegateCommand(ExecuteAnalyzeAndCompileCommand));
        #endregion

        public PatternCompilerViewModel(
            IContainerProvider containerProvider,
             IEventAggregator eventAggregator,
             IPatternCompilerBLL patternCompilerBLL) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _patternCompilerBLL = patternCompilerBLL;
        }

        private void ClearPatternFile()
        {
            _patternFiles = new string[] { };
            PatternFilePath = string.Empty;
        }

        private void ExecutePatternFileBrowseCommand()
        {
            if (PatternFileIsDir)
            {
                var openDirDialog = new OpenFolderDialog();
                if (openDirDialog.ShowDialog() == true)
                {
                    PatternFilePath = openDirDialog.FolderName;
                    var files = Directory.GetFiles(PatternFilePath, "*.atp");
                    _patternFiles = files;
                }
            }
            else
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = ".atp文件|*.atp";
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == true)
                {
                    PatternFilePath = openFileDialog.FileName;
                    _patternFiles = openFileDialog.FileNames;
                }
            }
        }

        private void ClearTestPlan()
        {
            _testPlanFiles = new string[] { };
            _testPlanName = string.Empty;
        }

        private void ExecuteTestPlanBrowseCommand()
        {
            if (TestPlanIsDir)
            {
                var openDirDialog = new OpenFolderDialog();
                if (openDirDialog.ShowDialog() == true)
                {
                    TestPlanName = openDirDialog.FolderName;
                    var files = Directory.GetFiles(TestPlanName, "*.xlsm");
                    _testPlanFiles = files;
                }
            }
            else
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "测试计划|*.xlsm;*.csv";
                if (openFileDialog.ShowDialog() == true)
                {
                    TestPlanName = openFileDialog.FileName;
                    _testPlanFiles = openFileDialog.FileNames;
                }
            }
        }

        private void ExecuteOutputBrowseCommand()
        {
            var openDirDialog = new OpenFolderDialog();
            if (openDirDialog.ShowDialog() == true)
                OutputDir = openDirDialog.FolderName;
        }

        private async Task ExecuteCompilerCommand()
        {
            if (_patternFiles.IsEmpty())
                return;

            if (_patternFileIsDir)
            {

            }
            else
            {
                var patternFile = _patternFiles.FirstOrDefault();
                if (!patternFile.IsEmpty() && File.Exists(patternFile))
                {
                    var patternModel = await _patternCompilerBLL.AnalysisPattern(patternFile);
                    _eventAggregator.GetEvent<PatternModelUpdateEvent>().Publish(patternModel);
                }
            }
            _eventAggregator.GetEvent<MessageOpenEvent>().Publish();
        }

        private async void ExecuteAnalyzeAndCompileCommand()
        {
            if (_patternFiles.IsEmpty())
                return;

            if (_patternFileIsDir)
            {

            }
            else
            {
                var patternFile = _patternFiles.FirstOrDefault();
                if (!patternFile.IsEmpty() && File.Exists(patternFile))
                {
                    _eventAggregator.GetEvent<MessageOpenEvent>().Publish();

                    await Task.Delay(500);

                    Task.Run(async () => _patternCompilerBLL.AnalyzeAndCompilePatternAsync(patternFile));
                }
            }
        }
    }
}
