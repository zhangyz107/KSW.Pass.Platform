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
using KSW.ATE01.Pattern.Application.Models.Projects;
using KSW.ATE01.Pattern.Domain.Projects.Core.Enums;
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

        private PatternInfoModel _patternItem;
        #endregion

        #region Properties
        public ObservableCollection<PatternInfoModel> PatternInfos { get; private set; } = new ObservableCollection<PatternInfoModel>();

        public PatternInfoModel PatternItem
        {
            get => _patternItem;
            set => SetProperty(ref _patternItem, value);
        }
        #endregion

        #region Command
        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));

        private DelegateCommand _addVectorCommand;
        public DelegateCommand AddVectorCommand =>
            _addVectorCommand ?? (_addVectorCommand = new DelegateCommand(ExecuteAddVectorCommand));

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

            PatternInfos.CollectionChanged += PatternInfos_CollectionChanged;
        }

        private void PatternInfos_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var index = 0;
            foreach (var item in PatternInfos)
                item.Vector = ++index;
        }

        private void ExecuteLoadingCommand()
        {
            var length = 10;
            for (int i = 0; i < length; i++)
            {
                var tempPattern = new PatternInfoModel();
                tempPattern.Vector = i + 1;
                tempPattern.Label = $"Label{i}";
                tempPattern.Command = new CommandInfoModel()
                {
                    Type = CommandType.repeat,
                };
                tempPattern.Instrument = $"Instrument{i}";
                tempPattern.TimingName = "time_fun";
                tempPattern.PinInfos = GetPinInfos();

                PatternInfos.Add(tempPattern);
            }

            var pattern = PatternInfos.FirstOrDefault();

            _eventAggregator?.GetEvent<PatternColInfoUpdateEvent>().Publish(pattern);
        }

        private List<PinInfoModel> GetPinInfos()
        {
            var result = new List<PinInfoModel>();
            var length = 16;
            for (int i = 0; i < length; i++)
            {
                var pinInfo = new PinInfoModel()
                {
                    PinName = $"A{i + 1}",
                    VectorValue = VectorValueType.X
                };
                result.Add(pinInfo);
            }

            return result;
        }

        private void ExecuteAddVectorCommand()
        {
            PatternInfos.Add(new PatternInfoModel());
        }

        private void ExecuteInsertVectorCommand(object obj)
        {
            if (obj is PatternInfoModel item)
            {
                var index = PatternInfos.IndexOf(item);
                PatternInfos.Insert(index + 1, new PatternInfoModel());
            }
        }

        private void ExecuteDeleteVectorCommand(object obj)
        {
            if (obj is PatternInfoModel item)
                PatternInfos.Remove(item);

            if (PatternItem != null)
                PatternInfos.Remove(PatternItem);
        }
    }
}
