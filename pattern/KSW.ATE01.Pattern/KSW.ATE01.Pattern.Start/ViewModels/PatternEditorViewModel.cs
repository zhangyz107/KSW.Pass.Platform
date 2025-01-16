/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：PatternEditorViewModel.cs
// 功能描述：向量编辑视图模型
//
// 作者：zhangyingzhong
// 日期：2025/01/10 15:33
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Patterns;
using KSW.ATE01.Pattern.Application.BLLs.Implements.Patterns;
using KSW.ATE01.Pattern.Application.Events;
using KSW.ATE01.Pattern.Application.Events.Patterns;
using KSW.ATE01.Pattern.Application.Models.Projects;
using KSW.Ui;
using System.Collections.ObjectModel;

namespace KSW.ATE01.Pattern.Start.ViewModels
{
    /// <summary>
    /// 向量编辑视图模型
    /// </summary>
    public class PatternEditorViewModel : ViewModelBase
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IPatternCompilerBLL _patternCompilerBLL;
        private PatternModel _patternModel;
        private PatternVectorModel _vectorRow;
        private bool _showPinOverview = true;
        #endregion

        #region Properties
        public ObservableCollection<PatternVectorModel> VectorInfos { get; private set; } = new ObservableCollection<PatternVectorModel>();

        public ObservableCollection<PinModel> PinCols { get; set; } = new ObservableCollection<PinModel>();

        public PatternVectorModel VectorRow
        {
            get => _vectorRow;
            set
            {
                if (SetProperty(ref _vectorRow, value))
                {
                    DeleteVectorCommand.RaiseCanExecuteChanged();
                }

            }
        }

        public bool ShowPinOverview
        {
            get => _showPinOverview;
            set => SetProperty(ref _showPinOverview, value);
        }

        #endregion

        #region Command
        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));

        private DelegateCommand _addVectorCommand;
        public DelegateCommand AddVectorCommand =>
            _addVectorCommand ?? (_addVectorCommand = new DelegateCommand(ExecuteAddVectorCommand, () => { return _patternModel != null; }));

        private DelegateCommand _openCommand;
        public DelegateCommand OpenCommand =>
             _openCommand ?? (_openCommand = new DelegateCommand(ExecuteOpenCommand));

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
             _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand, () => { return _patternModel != null; }));

        private DelegateCommand _saveAsCommand;
        public DelegateCommand SaveAsCommand =>
             _saveAsCommand ?? (_saveAsCommand = new DelegateCommand(ExecuteSaveAsCommand, () => { return _patternModel != null; }));

        private DelegateCommand _exportAtpCommand;
        public DelegateCommand ExportAtpCommand =>
             _exportAtpCommand ?? (_exportAtpCommand = new DelegateCommand(ExecuteExportAtpCommand, () => { return _patternModel != null; }));

        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));

        private DelegateCommand _pinOverviewVisibleCommand;
        public DelegateCommand PinOverviewVisibleCommand =>
            _pinOverviewVisibleCommand ?? (_pinOverviewVisibleCommand = new DelegateCommand(ExecutePinOverviewVisibleCommand));

        private DelegateCommand<object> _insertVectorCommand;
        public DelegateCommand<object> InsertVectorCommand =>
            _insertVectorCommand ?? (_insertVectorCommand = new DelegateCommand<object>(ExecuteInsertVectorCommand, (x) => { return _patternModel != null; }));

        private DelegateCommand<object> _deleteVectorCommand;
        public DelegateCommand<object> DeleteVectorCommand =>
            _deleteVectorCommand ?? (_deleteVectorCommand = new DelegateCommand<object>(ExecuteDeleteVectorCommand, (x) => { return _vectorRow != null; }));
        #endregion

        public PatternEditorViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IPatternCompilerBLL patternCompilerBLL) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _patternCompilerBLL = patternCompilerBLL;

            eventAggregator.GetEvent<PatternModelUpdateEvent>().Subscribe(PatternModelUpdate, ThreadOption.UIThread);
            VectorInfos.CollectionChanged += PatternInfos_CollectionChanged;
        }

        private void ExecuteLoadingCommand()
        {
            AddVectorCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            SaveAsCommand.RaiseCanExecuteChanged();
            ExportAtpCommand.RaiseCanExecuteChanged();
            InsertVectorCommand.RaiseCanExecuteChanged();
            DeleteVectorCommand.RaiseCanExecuteChanged();
        }

        private void PatternModelUpdate(PatternModel model)
        {
            if (model == null)
                return;

            if (model.PatternVectors.IsEmpty())
                return;

            _patternModel = model;
            VectorInfos.Clear();
            _eventAggregator?.GetEvent<PatternColInfoUpdateEvent>().Publish(model);

            ExecuteRefreshCommand();

            foreach (var item in model.PatternVectors)
                VectorInfos.Add(item);
        }

        private void PatternInfos_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var index = 0;
            foreach (var item in VectorInfos)
                item.Label.IndexInVectors = ++index;
        }
        //private List<PinModel> GetPinInfos()
        //{
        //    var result = new List<PinModel>();
        //    var length = 16;
        //    for (int i = 0; i < length; i++)
        //    {
        //        var pinInfo = new PinModel()
        //        {
        //            PinName = $"A{i + 1}",
        //            VectorValue = VectorValueType.X
        //        };
        //        result.Add(pinInfo);
        //    }

        //    return result;
        //}

        private void ExecuteAddVectorCommand()
        {
            VectorInfos.Add(new PatternVectorModel()
            {
                Label = new LabelModel(),
                Command = new CommandModel()
            });
        }

        private void ExecuteOpenCommand()
        {

        }

        private void ExecuteSaveCommand()
        {

        }

        private void ExecuteSaveAsCommand()
        {

        }

        private void ExecuteExportAtpCommand()
        {

        }

        private void ExecuteRefreshCommand()
        {
            if (_patternModel == null)
                return;

            if (_patternModel.PatternVectors.IsEmpty())
                return;

            PinCols.Clear();
            var tempRow = _patternModel?.PatternVectors?.FirstOrDefault();

            foreach (var item in tempRow.Pins)
                PinCols.Add(item);

        }

        private void ExecutePinOverviewVisibleCommand()
        {
            ShowPinOverview = !_showPinOverview;
        }

        private void ExecuteInsertVectorCommand(object obj)
        {
            if (obj is PatternVectorModel item)
            {
                var index = VectorInfos.IndexOf(item);
                VectorInfos.Insert(index + 1, new PatternVectorModel());
            }
        }

        private void ExecuteDeleteVectorCommand(object obj)
        {
            if (obj is PatternVectorModel item)
                VectorInfos.Remove(item);

            if (VectorRow != null)
                VectorInfos.Remove(VectorRow);
        }
    }
}
