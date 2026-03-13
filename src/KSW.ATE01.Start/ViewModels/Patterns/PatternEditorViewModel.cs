using KSW.ATE01.Application.BLLs.Abstractions.Patterns;
using KSW.ATE01.Application.Events;
using KSW.ATE01.Application.Models.Patterns;
using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.ATE01.Start.Views.Patterns;
using KSW.Dtos;
using KSW.Helpers;
using KSW.Ui;
using KSW.UI.WPF.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Start.ViewModels.Patterns
{
    public class PatternEditorViewModel : ViewModelBase, INavigationAware
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IPatternBLL _patternBLL;
        private string _title;
        private PatternModel _pattern;
        private PatternVectorModel _vectorRow;
        private PatternEditorView _view;
        private bool _isShowPinOverview = true;
        private bool _isInit = true;
        public event EventHandler<IEnumerable<PatternVectorModel>> PatternUpdated;

        public ObservableCollection<PatternVectorModel> VectorInfos { get; private set; } = new ObservableCollection<PatternVectorModel>();

        public ObservableCollection<PinModel> PinCols { get; set; } = new ObservableCollection<PinModel>();
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public PatternModel Pattern
        {
            get => _pattern;
            set => SetProperty(ref _pattern, value);
        }

        /// <summary>
        /// 是否显示引脚概览
        /// </summary>
        public bool IsShowPinOverview
        {
            get => _isShowPinOverview;
            set => SetProperty(ref _isShowPinOverview, value);
        }

        /// <summary>
        /// 向量行
        /// </summary>
        public PatternVectorModel VectorRow
        {
            get => _vectorRow;
            set => SetProperty(ref _vectorRow, value);
        }

        #region Commands

        /// <summary>
        /// 加载命令
        /// </summary>
        private DelegateCommand<object> _loadingCommand;
        public DelegateCommand<object> LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand<object>(ExecuteLoadingCommand));

        /// <summary>
        /// 刷新命令
        /// </summary>
        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));

        /// <summary>
        /// 保存命令
        /// </summary>
        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand));

        /// <summary>
        /// 另存为命令
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

        #endregion

        public PatternEditorViewModel(
            IEventAggregator eventAggregator,
            IContainerProvider containerProvider,
            IPatternBLL patternBLL) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _patternBLL = patternBLL;

            VectorInfos.CollectionChanged += VectorInfos_CollectionChanged;
        }


        private void ExecuteLoadingCommand(object parameter)
        {
            if (parameter is PatternEditorView view)
            {
                _view = view;
            }
        }

        private void VectorInfos_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Replace && !_isInit)
            {
                var index = 0;
                foreach (var item in VectorInfos)
                    item.Label.IndexInVectors = ++index;
            }
        }

        private void ExecuteRefreshCommand()
        {
            PinCols.Clear();
            var tempRow = Pattern?.PatternVectors?.FirstOrDefault();

            foreach (var item in tempRow.Pins)
                PinCols.Add(item);
        }

        private async void ExecuteSaveCommand()
        {
            Pattern.PatternVectors = VectorInfos.ToList();

            var result = await _patternBLL?.SavePattern(Pattern, Pattern.FilePath);
            if (result)
                _eventAggregator.GetEvent<ShowShellToastEvent>().Publish(new Toast()
                {
                    Content = L["SaveSuccessful"],
                    Type = UI.WPF.Enums.NotificationType.Success,
                });
        }

        private async void ExecuteSaveAsCommand()
        {
            var fileSaveDialog = new SaveFileDialog();
            fileSaveDialog.Filter = ".atp文件|*.atp";
            if (fileSaveDialog.ShowDialog() == true)
            {
                var filePath = fileSaveDialog.FileName;
                Pattern.PatternVectors = VectorInfos.ToList();
                var result = await _patternBLL?.SavePattern(Pattern, filePath);
                if (result)
                    _eventAggregator.GetEvent<ShowShellToastEvent>().Publish(new Toast()
                    {
                        Content = L["SaveSuccessful"],
                        Type = UI.WPF.Enums.NotificationType.Success,
                    });
            }
        }

        private async void ExecuteCompileCommand()
        {
            if (!IsVaild(VectorInfos,out var error))
            {
                _eventAggregator.GetEvent<ShowShellToastEvent>().Publish(new Toast()
                {
                    Content = error,
                    Type = UI.WPF.Enums.NotificationType.Error,
                });
                return ;
            }

            var fileSaveDialog = new SaveFileDialog();
            fileSaveDialog.Filter = ".bin文件|*.bin";
            fileSaveDialog.FileName = $"{Pattern.FileName}.bin";
            if (fileSaveDialog.ShowDialog() == true)
            {
                var filePath = fileSaveDialog.FileName;
                Pattern.PatternVectors = VectorInfos.ToList();
                var result = await _patternBLL?.CompileAsync(Pattern, filePath);
                if (result)
                    _eventAggregator.GetEvent<ShowShellToastEvent>().Publish(new Toast()
                    {
                        Content = L["CompileSuccessful"],
                        Type = UI.WPF.Enums.NotificationType.Success,
                    });
                else
                    _eventAggregator.GetEvent<ShowShellToastEvent>().Publish(new Toast()
                    {
                        Content = L["CompileFailed"],
                        Type = UI.WPF.Enums.NotificationType.Error,
                    });
            }

        }

        private bool IsVaild(IEnumerable<PatternVectorModel> list,out string error)
        {
            var hasError = list.FirstOrDefault(x => !x.Error.IsEmpty() || x.Pins.Any(y=>!y.Error.IsEmpty()));
            error = hasError?.Error ?? string.Empty;
            return hasError == null;
        }

        private void ExecuteInsertVectorCommand()
        {
            var index = _vectorRow == null ? VectorInfos.Count : VectorInfos.IndexOf(VectorRow);
            var newVector = new PatternVectorModel()
            {
                Label = new LabelModel(),
                Command = new CommandModel(),
                Pins = new List<PinModel>()
            };
            var vector = _vectorRow ?? VectorInfos.FirstOrDefault();
            ClonePins(vector, newVector);
            if (index >= 0)
                VectorInfos.Insert(index, newVector);
        }

        private void ExecuteAddVectorCommand()
        {
            var newVector = new PatternVectorModel()
            {
                Label = new LabelModel(),
                Command = new CommandModel(),
                Pins = new List<PinModel>()
            };
            var vector = _vectorRow ?? VectorInfos.FirstOrDefault();
            ClonePins(vector, newVector);
            VectorInfos.Add(newVector);
        }

        private void ClonePins(PatternVectorModel? source, PatternVectorModel target)
        {
            if (target.Pins == null)
                target.Pins = new List<PinModel>();

            foreach (var item in source.Pins)
            {
                var newPin = new PinModel()
                {
                    PinName = item.PinName,
                    VectorValue = VectorValueType.Zero,
                };
                target.Pins.Add(newPin);
            }
        }

        private void ExecuteRemoveVectorCommand()
        {
            var index = _vectorRow == null ? VectorInfos.Count : VectorInfos.IndexOf(VectorRow);
            if (index >= 0)
                VectorInfos.RemoveAt(index);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            var pattern = navigationContext.Parameters.GetValue<PatternModel>("pattern");
            if (pattern == null)
                return true;
            else
                return _pattern != null && _pattern.FileName == pattern.FileName;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var pattern = navigationContext.Parameters.GetValue<PatternModel>("pattern");
            if (pattern != null)
            {
                VectorInfos.Clear();
                Pattern = pattern;
                Title = Path.GetFileNameWithoutExtension(pattern.FileName);
                if (!Pattern.PatternVectors.IsEmpty())
                {
                    var index = 0;
                    foreach (var item in Pattern.PatternVectors)
                    {
                        item.Label.IndexInVectors = ++index;
                        VectorInfos.Add(item);
                    }
                }
                PatternUpdated?.Invoke(this, VectorInfos);
                ExecuteRefreshCommand();
            }
            _isInit = false;
        }


    }
}
