/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：PatternEditorViewModel.cs
// 功能描述：向量编辑视图模型
//
// 作者：zhangyingzhong
// 日期：2026/02/03 14:42
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions.Patterns;
using KSW.ATE01.Application.Models.Patterns;
using KSW.ATE01.Start.Views.Patterns;
using KSW.Helpers;
using KSW.Ui;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Start.ViewModels.Patterns
{
    /// <summary>
    /// 向量编辑视图模型
    /// </summary>
    public class PatternToolViewModel : ViewModelBase
    {
        #region Fields
        private readonly IPatternBLL _patternBLL;
        private readonly IRegionManager _regionManager;
        #endregion

        #region Properties
        /// <summary>
        /// 向量文件集合
        /// </summary>
        public ObservableCollection<PatternModel> Patterns { get; private set; } = new ObservableCollection<PatternModel>();

        private object _currentPattern;
        /// <summary>
        /// 当前向量
        /// </summary>
        public object CurrentPattern
        {
            get => _currentPattern;
            set => SetProperty(ref _currentPattern, value);
        }

        #endregion

        #region Commands
        /// <summary>
        /// 加载命令
        /// </summary>
        private DelegateCommand _openFileCommand;
        public DelegateCommand OpenFileCommand =>
            _openFileCommand ?? (_openFileCommand = new DelegateCommand(ExecuteOpenFileCommand));

        /// <summary>
        /// 加载命令
        /// </summary>
        private DelegateCommand _openDirCommand;
        public DelegateCommand OpenDirCommand =>
            _openDirCommand ?? (_openDirCommand = new DelegateCommand(ExecuteOpenDirCommand));

        /// <summary>
        /// 保存命令
        /// </summary>
        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand));

        /// <summary>
        /// 另保存命令
        /// </summary>
        private DelegateCommand _saveAsCommand;
        public DelegateCommand SaveAsCommand =>
            _saveAsCommand ?? (_saveAsCommand = new DelegateCommand(ExecuteSaveAsCommand));

        /// <summary>
        /// 编译命令
        /// </summary>
        private DelegateCommand _compileCommand;
        public DelegateCommand CompileCommand =>
            _compileCommand ?? (_compileCommand = new DelegateCommand(ExecuteCompileCommand));

        /// <summary>
        /// 插入向量命令
        /// </summary>
        private DelegateCommand _insertVectorCommand;
        public DelegateCommand InsertVectorCommand =>
            _insertVectorCommand ?? (_insertVectorCommand = new DelegateCommand(ExecuteInsertVectorCommand));

        /// <summary>
        /// 添加向量命令
        /// </summary>
        private DelegateCommand _addVectorCommand;
        public DelegateCommand AddVectorCommand =>
            _addVectorCommand ?? (_addVectorCommand = new DelegateCommand(ExecuteAddVectorCommand));

        /// <summary>
        /// 删除向量命令
        /// </summary>
        private DelegateCommand _removeVectorCommand;
        public DelegateCommand RemoveVectorCommand =>
            _removeVectorCommand ?? (_removeVectorCommand = new DelegateCommand(ExecuteRemoveVectorCommand));

        /// <summary>
        /// 清除Tab命令
        /// </summary>
        public DelegateCommand ClearTabCommand { get; set; }
        #endregion

        public PatternToolViewModel(
            IContainerProvider containerProvider,
            IPatternBLL patternBLL,
            IRegionManager regionManager) : base(containerProvider)
        {
            _patternBLL = patternBLL;
            _regionManager = regionManager;
        }

        private async void ExecuteOpenFileCommand()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pattern Files (*.atp)|*.atp";
            if (openFileDialog.ShowDialog() == true)
            {
                var processBarParameters = ProcessBarHelper.CreateProcessBarParameters(async (action) =>
                {
                    var patterns = await _patternBLL.GetPatternsByFilesAsync(openFileDialog.FileName);

                    if (!patterns.IsEmpty())
                    {
                        Patterns.Clear();
                        Patterns.AddRange(patterns);
                        UpdateTabControl();
                    }
                });

                await ProcessBarHelper.ShowProcessBarDialogAsync(DialogService, processBarParameters);

            }

        }

        /// <summary>
        /// 更新标签页
        /// </summary>
        private void UpdateTabControl()
        {
            if (!Patterns.IsEmpty())
            {
                ClearTabCommand?.Execute();
                var reverse = Patterns.Reverse();
                foreach (var item in reverse)
                {
                    var parameters = new NavigationParameters();
                    parameters.Add("pattern", item);
                    _regionManager.RequestNavigate(RegionNameManagement.PatternEditorContent, nameof(PatternEditorView), parameters);
                }
            }
        }

        private async void ExecuteOpenDirCommand()
        {
            var openFolderDialog = new OpenFolderDialog();
            if (openFolderDialog.ShowDialog() == true)
            {
                var dir = openFolderDialog.FolderName;
                var stopwatch = new Stopwatch();
                var processBarParameters = ProcessBarHelper.CreateProcessBarParameters(async (action) =>
                {
                    stopwatch.Start();
                    var patterns = await _patternBLL.GetPatternsByFilesAsync(dir, true);
                    if (!patterns.IsEmpty())
                    {
                        Patterns.Clear();
                        Patterns.AddRange(patterns);
                        UpdateTabControl();
                    }
                    stopwatch.Stop();
                    Debug.WriteLine($"耗时：{stopwatch.ElapsedMilliseconds}ms");
                });

                await ProcessBarHelper.ShowProcessBarDialogAsync(DialogService, processBarParameters);
            }
        }

        private void ExecuteSaveCommand()
        {
            if (_currentPattern != null && _currentPattern is PatternEditorView editorView)
            {
                var model = editorView.DataContext as PatternEditorViewModel;
                if (model != null)
                    model?.SaveCommand?.Execute();
            }
        }

        private void ExecuteSaveAsCommand()
        {
            if (_currentPattern != null && _currentPattern is PatternEditorView editorView)
            {
                var model = editorView.DataContext as PatternEditorViewModel;
                if (model != null)
                    model?.SaveAsCommand?.Execute();
            }
        }

        private void ExecuteInsertVectorCommand()
        {
            if (_currentPattern != null && _currentPattern is PatternEditorView editorView)
            {
                var model = editorView.DataContext as PatternEditorViewModel;
                if (model != null)
                    model?.InsertVectorCommand?.Execute();
            }
        }

        private void ExecuteAddVectorCommand()
        {
            if (_currentPattern != null && _currentPattern is PatternEditorView editorView)
            {
                var model = editorView.DataContext as PatternEditorViewModel;
                if (model != null)
                    model?.AddVectorCommand?.Execute();
            }
        }

        private void ExecuteRemoveVectorCommand()
        {
            if (_currentPattern != null && _currentPattern is PatternEditorView editorView)
            {
                var model = editorView.DataContext as PatternEditorViewModel;
                if (model != null)
                    model?.RemoveVectorCommand?.Execute();
            }
        }

        private void ExecuteCompileCommand()
        {
            if (_currentPattern != null && _currentPattern is PatternEditorView editorView)
            {
                var model = editorView.DataContext as PatternEditorViewModel;
                if (model != null)
                    model?.CompileCommand?.Execute();
            }
        }
    }
}
