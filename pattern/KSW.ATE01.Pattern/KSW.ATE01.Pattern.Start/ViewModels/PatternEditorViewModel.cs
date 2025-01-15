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
        private PatternModel _patternModel;
        private PatternVectorModel _vectorRow;
        #endregion

        #region Properties
        public ObservableCollection<PatternVectorModel> VectorInfos { get; private set; } = new ObservableCollection<PatternVectorModel>();

        public ObservableCollection<PinModel> PinCols { get; set; } = new ObservableCollection<PinModel>();

        public PatternVectorModel VectorRow
        {
            get => _vectorRow;
            set => SetProperty(ref _vectorRow, value);
        }
        #endregion

        #region Command
        private DelegateCommand _addVectorCommand;
        public DelegateCommand AddVectorCommand =>
            _addVectorCommand ?? (_addVectorCommand = new DelegateCommand(ExecuteAddVectorCommand));

        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));

        private DelegateCommand<object> _insertVectorCommand;
        public DelegateCommand<object> InsertVectorCommand =>
            _insertVectorCommand ?? (_insertVectorCommand = new DelegateCommand<object>(ExecuteInsertVectorCommand));

        private DelegateCommand<object> _deleteVectorCommand;
        public DelegateCommand<object> DeleteVectorCommand =>
            _deleteVectorCommand ?? (_deleteVectorCommand = new DelegateCommand<object>(ExecuteDeleteVectorCommand));
        #endregion

        public PatternEditorViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;

            eventAggregator.GetEvent<PatternModelUpdateEvent>().Subscribe(PatternModelUpdate, ThreadOption.UIThread);
            VectorInfos.CollectionChanged += PatternInfos_CollectionChanged;
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
            VectorInfos.Add(new PatternVectorModel());
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
