/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：RealTimeTxtViewModel.cs
// 功能描述：实时文本视图模型
//
// 作者：zhangyingzhong
// 日期：2024/12/17 16:08
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using DryIoc.ImTools;
using KSW.ATE01.Application.Events.RealTimeTxts;
using KSW.ATE01.Application.Helpers;
using KSW.ATE01.Application.Models.RealTimeTxt;
using KSW.ATE01.Start.Views.Dialogs;
using KSW.Ui;
using SixLabors.ImageSharp.ColorSpaces;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace KSW.ATE01.Start.ViewModels
{
    public class RealTimeTxtViewModel : ViewModelBase
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly string _logFilePath = "C:\\Users\\zhang\\Desktop\\1663tch.txt";
        private FlowDocument _flowDocument;
        private string _searchContent;
        private bool _isWholeWordMatch;
        private bool _isLoopSearch;
        private RichTextBox _richTextBox;
        private TextPointer _currentPointer;
        #endregion

        #region Properties
        public string Title => L["RealTimeTxt"];

        public string SearchContent
        {
            get => _searchContent;
            set => SetProperty(ref _searchContent, value);
        }

        public bool IsWholeWordMatch
        {
            get => _isWholeWordMatch;
            set => SetProperty(ref _isWholeWordMatch, value);
        }

        public bool IsLoopSearch
        {
            get => _isLoopSearch;
            set => SetProperty(ref _isLoopSearch, value);
        }

        public FlowDocument FlowDocument
        {
            get => _flowDocument;
            set => SetProperty(ref _flowDocument, value);
        }

        #endregion

        #region Command
        private DelegateCommand<object> _loadingCommand;
        public DelegateCommand<object> LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand<object>(ExecuteLoadingCommand));

        private DelegateCommand<object> _searchCommand;
        public DelegateCommand<object> SearchCommand =>
            _searchCommand ?? (_searchCommand = new DelegateCommand<object>(ExecuteSearchCommand));

        private DelegateCommand _clearAllCommand;
        public DelegateCommand ClearAllCommand =>
            _clearAllCommand ?? (_clearAllCommand = new DelegateCommand(ExecuteClearAllCommand));

        private DelegateCommand _reopenFileCommand;
        public DelegateCommand ReopenFileCommand =>
            _reopenFileCommand ?? (_reopenFileCommand = new DelegateCommand(ExecuteReopenFileCommand));

        private DelegateCommand<object> _toggleBookmarkCommand;
        public DelegateCommand<object> ToggleBookmarkCommand =>
            _toggleBookmarkCommand ?? (_toggleBookmarkCommand = new DelegateCommand<object>(ExecuteToggleBookmarkCommand));

        private DelegateCommand<object> _goToBookmarkCommand;
        public DelegateCommand<object> GoToBookmarkCommand =>
            _goToBookmarkCommand ?? (_goToBookmarkCommand = new DelegateCommand<object>(ExecuteGoToBookmarkCommand));

        private DelegateCommand _clearBookmarkCommand;
        public DelegateCommand ClearBookmarkCommand =>
            _clearBookmarkCommand ?? (_clearBookmarkCommand = new DelegateCommand(ExecuteClearBookmarkCommand));

        #endregion

        public RealTimeTxtViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
        }


        private void ExecuteLoadingCommand(object richTextBox)
        {
            if (richTextBox is RichTextBox richTB)
            {
                _richTextBox = richTB;
                _richTextBox.MouseRightButtonUp += RichTextBox_MouseRightButtonUp;
                LoadTextFile(richTB, _logFilePath);
            }
        }

        private void RichTextBox_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is RichTextBox richTextBox && e.RightButton == System.Windows.Input.MouseButtonState.Released)
            {
                var point = e.GetPosition(richTextBox);
                var textPointer = richTextBox.GetPositionFromPoint(point, true);
                if (textPointer != null)
                {
                    _currentPointer = textPointer;
                    var run = textPointer.Parent as Run;
                    Debug.WriteLine(run?.Name);
                    Debug.WriteLine(run?.Text);
                }
            }
        }

        private void LoadTextFile(RichTextBox richTB, string filePath)
        {
            if (richTB == null)
                return;

            richTB.Document.Blocks.Clear();
            // 打开文件并读取其内容
            var text = File.ReadAllText(filePath);
            var msgList = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (msgList.IsEmpty())
                return;

            Paragraph para = new Paragraph();
            foreach (var msg in msgList)
            {
                var hasMarker = false;
                if (msg.Contains("condition"))  //当出现标识字符时
                    hasMarker = true;
                Run r = new Run(msg);
                r.Name = $"run{msgList.IndexOf(msg)}";
                if (hasMarker)
                    r.Foreground = Brushes.Red;
                else
                    r.Foreground = Brushes.Black;
                para.Inlines.Add(r);
            }
            richTB.Document.Blocks.Add(para);
        }

        private void ExecuteSearchCommand(object isUpSearch)
        {
            var findResult = false;

            if (bool.TryParse(isUpSearch.ToString(), out bool searchType) && !SearchContent.IsEmpty())
            {
                if (searchType)
                {
                    var pos = new Point(10, 10);
                    var currentBeginPointer = _richTextBox.GetPositionFromPoint(pos, true);
                    var documentRange = new TextRange(_richTextBox.Document.ContentStart, currentBeginPointer);
                    var textToSearch = documentRange.Text;

                    // 在当前范围内搜索指定的文本
                    int index = textToSearch.LastIndexOf(SearchContent);
                    // 如果找到匹配的文本
                    if (index != -1)
                    {
                        TextPointer pointer = documentRange.Start.GetPositionAtOffset(index);

                        while (pointer != null && pointer.CompareTo(_richTextBox.Document.ContentEnd) < 0)
                        {
                            // 获取当前 TextPointer 指向的位置的字符
                            string currentText = pointer.GetTextInRun(LogicalDirection.Forward);

                            // 查找关键字
                            index = currentText.IndexOf(SearchContent, StringComparison.OrdinalIgnoreCase);
                            if (index > 0)
                            {
                                var run = pointer.Parent as Run;
                                // 找到匹配的关键字，选择文本并滚动
                                TextPointer startPointer = pointer.GetPositionAtOffset(index);
                                TextPointer endPointer = startPointer.GetPositionAtOffset(SearchContent.Length);

                                ScrollToSelection(startPointer, endPointer);

                                findResult = true;
                                break;
                            }
                            // 移动到下一个 TextPointer
                            pointer = pointer.GetPositionAtOffset(1);
                        }
                    }
                }
                else
                {
                    var pos = new Point(_richTextBox.ActualWidth, _richTextBox.ActualHeight);
                    TextPointer currentBeginPointer = _richTextBox.GetPositionFromPoint(pos, true);
                    var documentRange = new TextRange(currentBeginPointer, _richTextBox.Document.ContentEnd);
                    var textToSearch = documentRange.Text;

                    // 在当前范围内搜索指定的文本
                    int index = textToSearch.IndexOf(SearchContent);
                    // 如果找到匹配的文本
                    if (index != -1)
                    {
                        TextPointer pointer = documentRange.Start.GetPositionAtOffset(index);

                        while (pointer != null && pointer.CompareTo(_richTextBox.Document.ContentEnd) < 0)
                        {
                            // 获取当前 TextPointer 指向的位置的字符
                            string currentText = pointer.GetTextInRun(LogicalDirection.Forward);

                            // 查找关键字
                            index = currentText.IndexOf(SearchContent, StringComparison.OrdinalIgnoreCase);
                            if (index > 0)
                            {
                                var run = pointer.Parent as Run;
                                // 找到匹配的关键字，选择文本并滚动
                                TextPointer startPointer = pointer.GetPositionAtOffset(index);
                                TextPointer endPointer = startPointer.GetPositionAtOffset(SearchContent.Length);

                                ScrollToSelection(startPointer, endPointer);

                                findResult = true;
                                break;
                            }
                            // 移动到下一个 TextPointer
                            pointer = pointer.GetPositionAtOffset(1);
                        }
                    }
                }
              
                if (!findResult)
                {
                    MessageBox.Show("没有更多匹配的关键字。");
                }
            }
        }

        private void ExecuteClearAllCommand()
        {
            if (_richTextBox != null)
            {
                if (!File.Exists(_logFilePath))
                    return;

                FileStream stream = File.Open(_logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                stream.Seek(0, SeekOrigin.Begin);
                stream.SetLength(0);
                stream.Close();

                _richTextBox.Document.Blocks.Clear();
            }
        }

        private void ExecuteReopenFileCommand()
        {
            if (_richTextBox != null)
            {
                LoadTextFile(_richTextBox, _logFilePath);
            }
        }

        private void ExecuteToggleBookmarkCommand(object obj)
        {
            if (_richTextBox != null)
            {
                var selection = _richTextBox.Selection;
                // 获取选中内容的起始位置
                TextPointer startPointer = selection.Start;
                if (selection.IsEmpty)
                {
                    if (_currentPointer != null)
                    {
                        var run = _currentPointer.Parent as Run;
                        run.Foreground = Brushes.Yellow;
                        run.Background = Brushes.Green;
                        var runName = run.Name;
                        BookmarkManagerHelper.AddBookmark(runName, run.ContentStart, run.ContentEnd);
                    }
                }
                else
                {
                    // 获取选中内容的段落
                    Paragraph selectedParagraph = startPointer.Paragraph;
                    _richTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Yellow);
                    _richTextBox.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Green);
                    var run = startPointer.Parent as Run;
                    var runName = run.Name;
                    BookmarkManagerHelper.AddBookmark(runName, selection.Start, selection.End);
                }
            }
        }

        private void ExecuteGoToBookmarkCommand(object obj)
        {
            var isNext = true;
            bool.TryParse(obj.ToString(), out isNext);

            if (_richTextBox != null && _currentPointer != null)
            {
                var run = _currentPointer.Parent as Run;
                var runName = run.Name;

                var bookmarkPosition = BookmarkManagerHelper.GotoBookmark(runName, isNext);
                if (bookmarkPosition != null)
                {
                    TextPointer selectionStart = bookmarkPosition.Start;
                    TextPointer selectionEnd = bookmarkPosition.End;

                    ScrollToSelection(selectionStart, selectionEnd);
                }
            }
        }

        private ScrollViewer GetScrollViewer(DependencyObject obj)
        {
            if (obj is ScrollViewer)
                return (ScrollViewer)obj;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                var scrollViewer = GetScrollViewer(child);
                if (scrollViewer != null)
                    return scrollViewer;
            }
            return null;
        }

        private void ScrollToSelection(TextPointer selectionStart, TextPointer selectionEnd)
        {
            if (_richTextBox != null)
            {
                // 获取选中文本起始和结束位置的物理区域
                Rect selectionStartRect = selectionStart.GetCharacterRect(LogicalDirection.Forward);
                Rect selectionEndRect = selectionEnd.GetCharacterRect(LogicalDirection.Backward);

                // 计算选中文本区域的高度
                double selectionHeight = selectionEndRect.Bottom - selectionStartRect.Top;

                // 获取 RichTextBox 内部的 ScrollViewer 控件
                ScrollViewer scrollViewer = GetScrollViewer(_richTextBox);

                // 滚动到选中文本的位置，使其位于视窗的中间
                scrollViewer.ScrollToVerticalOffset(selectionStartRect.Top + scrollViewer.VerticalOffset - (selectionHeight / 2));
            }
        }

        private void ExecuteClearBookmarkCommand()
        {
            BookmarkManagerHelper.ClearBookmark();
            if (_richTextBox != null)
            {
                LoadTextFile(_richTextBox, _logFilePath);
            }
        }
    }
}
