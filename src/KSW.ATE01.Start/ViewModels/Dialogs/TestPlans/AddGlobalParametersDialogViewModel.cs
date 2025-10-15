/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：AddGlobalParametersDialogViewModel.cs
// 功能描述：添加全局参数对话框视图模型
//
// 作者：zhangyingzhong
// 日期：2025/08/25 09:56
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Events;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.Ui;
using KSW.UI.WPF.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace KSW.ATE01.Start.ViewModels.Dialogs.TestPlans
{
    /// <summary>
    /// 添加全局参数对话框视图模型
    /// </summary>
    public class AddGlobalParametersDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private readonly IGlobalParameterBLL _globalParameterBLL;
        private string _title;
        private ProjectInfoModel _projectInfo;
        private GlobalParameterModel _globalParameter;

        private ObservableCollection<AdditionalParameters> _parameterList = new ObservableCollection<AdditionalParameters>();
        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// 全局参数
        /// </summary>
        public GlobalParameterModel GlobalParameter
        {
            get => _globalParameter;
            set => SetProperty(ref _globalParameter, value);
        }

        /// <summary>
        /// 附加参数列表
        /// </summary>
        public ObservableCollection<AdditionalParameters> ParameterList
        {
            get => _parameterList;
            set => SetProperty(ref _parameterList, value);
        }
        #endregion

        #region Commands
        /// <summary>
        /// 添加附加参数命令
        /// </summary>
        private DelegateCommand _addCommand;
        public DelegateCommand AddCommand =>
            _addCommand ?? (_addCommand = new DelegateCommand(ExecuteAddCommand));

        /// <summary>
        /// 删除命令
        /// </summary>
        private DelegateCommand<AdditionalParameters> _deleteCommand;
        public DelegateCommand<AdditionalParameters> DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new DelegateCommand<AdditionalParameters>(ExecuteDeleteCommand));

        /// <summary>
        /// 确定命令
        /// </summary>
        private AsyncDelegateCommand _oKCommand;
        public AsyncDelegateCommand OKCommand =>
            _oKCommand ?? (_oKCommand = new AsyncDelegateCommand(ExecuteOKCommand, () => _globalParameter.Error.IsEmpty()));

        /// <summary>
        /// 取消命令
        /// </summary>
        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));
        #endregion

        public AddGlobalParametersDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IProjectBLL projectBLL,
            IGlobalParameterBLL globalParameterBLL) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _projectBLL = projectBLL;
            _globalParameterBLL = globalParameterBLL;
        }

        private void ExecuteAddCommand()
        {
            _parameterList.Add(new AdditionalParameters());
        }

        private void ExecuteDeleteCommand(AdditionalParameters parameters)
        {
            _parameterList.Remove(parameters);
        }

        private bool CheckInputValue()
        {
            var result = true;

            if (_globalParameter != null)
                result &= !_globalParameter.PatternFile.IsEmpty();

            return result;
        }

        private async Task ExecuteOKCommand()
        {
            try
            {
                var additionalParameters = _parameterList.Where(x => !x.Parameter.IsEmpty()).Select(x => x.Parameter);
                _globalParameter.AdditionInfo = string.Join(",", additionalParameters);

                if (_globalParameter.Id.IsEmpty())
                    await _globalParameterBLL?.CreateAsync(_globalParameter);
                else
                    await _globalParameterBLL?.UpdateAsync(_globalParameter);

                RaiseRequestClose(new DialogResult(ButtonResult.OK));
            }
            catch (Exception e)
            {
                _eventAggregator.GetEvent<ShowShellToastEvent>().Publish(new Toast()
                {
                    Content = e.Message,
                    Type = UI.WPF.Enums.NotificationType.Error,
                });
            }
        }

        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            var id = parameters.GetValue<string>("GlobalParameterId");
            var projectInfo = _projectBLL?.GetCurrentProjectInfo();
            _projectInfo = projectInfo;

            if (id == null)
            {
                Title = L["AddGlobalParameters"];
                GlobalParameter = new GlobalParameterModel()
                {
                    ProjectInfoId = _projectInfo?.Id.ToGuid(),
                };
            }
            else
            {
                Title = L["EditGlobalParameters"];
                GlobalParameter = await _globalParameterBLL?.GetByIdAsync(id);
            }

            if (_globalParameter != null)
            {
                _globalParameter.PropertyChanged += GlobalParameterPropertyChanged;
                if (!_globalParameter.AdditionInfo.IsEmpty())
                {
                    var additions = _globalParameter.AdditionInfo.Split(',');
                    foreach (var addition in additions)
                        _parameterList.Add(new AdditionalParameters() { Parameter = addition });
                }
            }
        }

        private void GlobalParameterPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OKCommand.RaiseCanExecuteChanged();
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }
    }
}
