/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：RealTimeTxtDialogViewModel.cs
// 功能描述：实时文本对话框视图模型
//
// 作者：zhangyingzhong
// 日期：2024/12/17 16:08
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.Events.RealTimeTxts;
using KSW.ATE01.Start.Views.Dialogs;
using KSW.Ui;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace KSW.ATE01.Start.ViewModels.Dialogs
{
    public class RealTimeTxtDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private bool _isFindDialogOpen = false;
        #endregion

        #region Properties
        public string Title => L["RealTimeTxt"];

        public DialogCloseListener RequestClose { get; }

        private FlowDocument _flowDocument;

        public FlowDocument FlowDocument
        {
            get => _flowDocument;
            set => SetProperty(ref _flowDocument, value);
        }

        #endregion

        #region Command

        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));

        private DelegateCommand _findCommand;

        public DelegateCommand FindCommand =>
            _findCommand ?? (_findCommand = new DelegateCommand(ExecuteFindCommand));
        #endregion

        public RealTimeTxtDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }

        private void ExecuteLoadingCommand()
        {

        }

        private void ExecuteFindCommand()
        {
            if (!_isFindDialogOpen)
            {
                _isFindDialogOpen = true;
                DialogService.Show(nameof(FindDialog), (result) =>
                {
                    _isFindDialogOpen = false;
                });
            }
        }
    }
}
