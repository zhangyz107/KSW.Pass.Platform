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

using KSW.ATE01.Application.BLLs.Abstractions.RealTimeTxt;
using KSW.ATE01.Application.Events.RealTimeTxts;
using KSW.ATE01.Application.Helpers;
using KSW.ATE01.Application.Models.RealTimeTxt;
using KSW.ATE01.Start.Views;
using KSW.ATE01.Start.Views.Dialogs;
using KSW.Helpers;
using KSW.Properties;
using KSW.Ui;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace KSW.ATE01.Start.ViewModels
{
    public class RealTimeTxtViewModel : ViewModelBase
    {
        #region Fields
        private static bool _isRefrash = false;
        private static object _lock = new object();
        private readonly IConfigureFileBLL _configureFileBLL;
        private readonly IEventAggregator _eventAggregator;
        private readonly string _logFilePath = "C:\\Users\\zhang\\Desktop\\1663tch.txt";
        private readonly string _prefixRun = "run";
        private ConfigureFileModel _configureFileModel;
        private string _searchContent;
        private bool _isWholeWordMatch;
        private bool _isLoopSearch;
        private bool _isPauseWindow = false;
        private RealTimeTxtView _view;
        private TextPointer _currentPointer;
        private TextRange _lastTextRange;
        private FileSystemWatcher _watcher;
        #endregion

        #region Properties
        public string Title => L["RealTimeTxt"];

        public string SearchContent
        {
            get => _searchContent;
            set
            {
                if (SetProperty(ref _searchContent, value))
                    SearchCommand.RaiseCanExecuteChanged();
            }
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

        public bool IsPauseWindow
        {
            get => _isPauseWindow;
            set => SetProperty(ref _isPauseWindow, value);
        }
        #endregion

        #region Command
        private DelegateCommand<object> _loadingCommand;
        public DelegateCommand<object> LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand<object>(ExecuteLoadingCommand));

        private DelegateCommand _unloadingCommand;
        public DelegateCommand UnloadingCommand =>
            _unloadingCommand ?? (_unloadingCommand = new DelegateCommand(ExecuteUnloadingCommand));

        private DelegateCommand _focusFindCommand;
        public DelegateCommand FocusFindCommand =>
            _focusFindCommand ?? (_focusFindCommand = new DelegateCommand(ExecuteFocusFindCommand));

        private AsyncDelegateCommand<object> _searchCommand;
        public AsyncDelegateCommand<object> SearchCommand =>
            _searchCommand ?? (_searchCommand = new AsyncDelegateCommand<object>(ExecuteSearchCommand, (x) => SearchContent.IsEmpty() == false));

        private DelegateCommand _clearAllCommand;
        public DelegateCommand ClearAllCommand =>
            _clearAllCommand ?? (_clearAllCommand = new DelegateCommand(ExecuteClearAllCommand));

        private DelegateCommand<object> _goToHighlightCommand;
        public DelegateCommand<object> GoToHighlightCommand =>
            _goToHighlightCommand ?? (_goToHighlightCommand = new DelegateCommand<object>(ExecuteGoToHighlightCommand));

        private DelegateCommand _pauseCommand;
        public DelegateCommand PauseCommand =>
            _pauseCommand ?? (_pauseCommand = new DelegateCommand(ExecutePauseCommand));

        private DelegateCommand _viewOptionsCommand;
        public DelegateCommand ViewOptionsCommand =>
            _viewOptionsCommand ?? (_viewOptionsCommand = new DelegateCommand(ExecuteViewOptionsCommand));

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
            IConfigureFileBLL configureFileBLL,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _configureFileBLL = configureFileBLL;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<ConfigureFileUpdateEvent>().Subscribe(ConfigureFileUpdate, ThreadOption.UIThread);
        }

        private void ConfigureFileUpdate()
        {
            _configureFileModel = _configureFileBLL.GetConfigureFile();

            LoadTextFile(_view?.richTB, _logFilePath);
        }

        private void ExecuteLoadingCommand(object richTextBox)
        {
            if (richTextBox is RealTimeTxtView view)
            {
                _view = view;
                var richTb = _view.richTB;
                _configureFileModel = _configureFileBLL.GetConfigureFile();
                if (richTb != null)
                {
                    richTb.MouseRightButtonUp += RichTextBox_MouseRightButtonUp;
                    richTb.SelectionChanged += RichTextBox_SelectionChanged;
                    LoadTextFile(richTb, _logFilePath);
                }

                _watcher = new FileSystemWatcher();
                _watcher.Path = Path.GetDirectoryName(_logFilePath);
                _watcher.Filter = Path.GetFileName(_logFilePath);
                _watcher.InternalBufferSize = 64 * 1024; // 设置为 64 KB
                _watcher.NotifyFilter = NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.Security
                | NotifyFilters.Size;
                _watcher.Changed += Watcher_Changed;
                _watcher.EnableRaisingEvents = true;
            }
        }

        private async void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.Equals(_logFilePath))
            {
                await Task.Delay(_configureFileModel.FileChangeInterval);

                await System.Windows.Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, () =>
                {
                    LoadTextFile(_view?.richTB, _logFilePath);
                });
            }
        }

        private void ExecuteUnloadingCommand()
        {
            if (_watcher != null)
                _watcher.EnableRaisingEvents = false;
        }

        private void RichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is RichTextBox richTextBox)
            {
                _lastTextRange = new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End);
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
            var msgList = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            if (msgList.IsEmpty())
                return;

            if (!_isRefrash)
            {
                lock (_lock)
                {
                    if (!_isRefrash)
                    {
                        _isRefrash = true;

                        var bookmarks = BookmarkManagerHelper.Bookmarks;
                        HighlightManagerHelper.ClearHighlight();

                        Paragraph para = new Paragraph();
                        var index = 0;
                        foreach (var msg in msgList)
                        {
                            //处理高亮字符串
                            var hasMarker = IsContainKeyword(msg, out bool addHighlight, out Color foreground);

                            var rowMsg = msg + "\r\n";
                            Run r = new Run(rowMsg);
                            r.Name = $"{_prefixRun}{index++}";
                            var hasBookmark = bookmarks.Any(x => x.RunName.Equals(r.Name));

                            //标记书签
                            if (hasBookmark)
                            {
                                r.Foreground = Brushes.Yellow;
                                r.Background = Brushes.Green;
                            }
                            else
                            {
                                //进行标记
                                if (hasMarker)
                                    r.Foreground = new SolidColorBrush(foreground);
                                else
                                    r.Foreground = Brushes.Black;
                            }

                            if (hasBookmark || addHighlight)
                                HighlightManagerHelper.AddHighlight(_prefixRun, r.Name);

                            para.Inlines.Add(r);
                        }
                        richTB.Document.Blocks.Add(para);
                        if (!_isPauseWindow)
                            richTB.ScrollToEnd();

                        _isRefrash = false;
                    }
                }
            }
        }

        private bool IsContainKeyword(string msg, out bool addHighlight, out Color foreground)
        {
            var result = false;
            addHighlight = false;
            foreground = Colors.Red;

            if (_configureFileModel.Keywords.IsEmpty())
                return result;

            foreach (var keyword in _configureFileModel.Keywords)
            {
                var isContain = msg.IndexOf(keyword.Keyword, StringComparison.OrdinalIgnoreCase);
                if (isContain >= 0)
                {
                    result = true;
                    addHighlight = keyword.IsHighlight == true;
                    if (addHighlight)
                        foreground = keyword.Foreground ?? Colors.Red;
                }

                if (result && addHighlight)
                    break;
            }

            return result;
        }

        private void ExecuteFocusFindCommand()
        {
            _view?.findTb?.Focus();
        }

        private async Task ExecuteSearchCommand(object isUpSearch)
        {
            var findResult = false;
            string pattern = $@"\b{System.Text.RegularExpressions.Regex.Escape(SearchContent)}\b"; // \b是单词边界
            var regex = new System.Text.RegularExpressions.Regex(pattern, RegexOptions.IgnoreCase);
            var documentRange = new TextRange(_view?.richTB?.Document?.ContentStart, _view?.richTB?.Document?.ContentEnd);
            //获取上次选择区域起止点
            var startPos = _lastTextRange?.Start;
            var endPos = _lastTextRange?.End;

            if (bool.TryParse(isUpSearch.ToString(), out bool searchType) && !SearchContent.IsEmpty())
            {
                if (searchType)
                {
                    if (startPos != null)
                        documentRange = new TextRange(_view?.richTB?.Document?.ContentStart, startPos);
                    var textToSearch = documentRange.Text;
                    findResult = SearchTarget(searchType, regex, documentRange, textToSearch);
                }
                else
                {
                    if (endPos != null)
                        documentRange = new TextRange(endPos, _view?.richTB?.Document?.ContentEnd);
                    var textToSearch = documentRange.Text;
                    findResult = SearchTarget(searchType, regex, documentRange, textToSearch);
                }

                if (!findResult)
                {
                    if (IsLoopSearch)
                    {
                        documentRange = new TextRange(_view?.richTB?.Document?.ContentStart, _view?.richTB?.Document?.ContentEnd);
                        var textToSearch = documentRange.Text;
                        findResult = SearchTarget(searchType, regex, documentRange, textToSearch);
                    }
                    else
                        await DialogService.ShowMessageDialog(L["MatchFailed"], MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            _view?.richTB?.Focus();
        }

        private bool SearchTarget(bool isSearchUp, System.Text.RegularExpressions.Regex regex, TextRange documentRange, string textToSearch)
        {
            var findResult = false;
            var flag = IsWholeWordMatch ? regex.IsMatch(textToSearch) : true;
            // 在当前范围内搜索指定的文本
            int index = isSearchUp ? textToSearch.LastIndexOf(SearchContent, StringComparison.OrdinalIgnoreCase) : textToSearch.IndexOf(SearchContent, StringComparison.OrdinalIgnoreCase);
            // 如果找到匹配的文本
            if (index != -1 && flag)
            {
                TextPointer pointer = documentRange.Start.GetPositionAtOffset(index);

                while (pointer != null && pointer.CompareTo(_view?.richTB?.Document?.ContentEnd) < 0)
                {
                    // 获取当前 TextPointer 指向的位置的字符
                    string currentText = pointer.GetTextInRun(LogicalDirection.Forward);
                    // 查找关键字
                    index = currentText.IndexOf(SearchContent, StringComparison.OrdinalIgnoreCase);
                    if (index >= 0)
                    {
                        var run = pointer.Parent as Run;
                        // 找到匹配的关键字，选择文本并滚动
                        TextPointer startPointer = pointer.GetPositionAtOffset(index);
                        TextPointer endPointer = startPointer.GetPositionAtOffset(SearchContent.Length);
                        ScrollToSelection(startPointer, endPointer);
                        _view?.richTB?.Selection?.Select(startPointer, endPointer);
                        findResult = true;
                        break;
                    }
                    // 移动到下一个 TextPointer
                    pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
                }
            }

            return findResult;
        }

        private void ExecuteClearAllCommand()
        {
            if (_view?.richTB != null)
            {
                if (!File.Exists(_logFilePath))
                    return;

                FileStream stream = File.Open(_logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                stream.Seek(0, SeekOrigin.Begin);
                stream.SetLength(0);
                stream.Close();

                _view?.richTB?.Document.Blocks.Clear();
            }
        }


        private void ExecuteGoToHighlightCommand(object obj)
        {
            var isNext = true;
            bool.TryParse(obj.ToString(), out isNext);

            if (_view?.richTB != null && _currentPointer != null)
            {
                var run = _currentPointer.Parent as Run;
                var runName = run.Name;

                var highlightPosition = HighlightManagerHelper.GotoHighlight(_prefixRun, runName, isNext);
                if (highlightPosition != null)
                {
                    // 获取 RichTextBox 中的所有 TextElements
                    var targetRun = _view?.richTB?.Document?.Blocks
                                    .OfType<Paragraph>()
                                    .SelectMany(p => p.Inlines.OfType<Run>())
                                    .Where(run => run.Name.Equals(highlightPosition.RunName))
                                    .FirstOrDefault();

                    TextPointer selectionStart = targetRun?.ContentStart;
                    TextPointer selectionEnd = targetRun?.ContentEnd;

                    if (selectionStart == null || selectionEnd == null)
                        return;

                    ScrollToSelection(selectionStart, selectionEnd);
                    _view?.richTB?.Selection?.Select(selectionStart, selectionEnd);
                    _currentPointer = isNext ? selectionEnd : selectionStart;
                }
            }
        }


        private void ExecutePauseCommand()
        {
            _watcher.EnableRaisingEvents = !_isPauseWindow;
        }

        private void ExecuteViewOptionsCommand()
        {
            DialogService.ShowDialog(nameof(ConfigureDialog));
        }

        private void ExecuteReopenFileCommand()
        {
            if (_view?.richTB != null)
            {
                LoadTextFile(_view?.richTB, _logFilePath);
            }
        }

        private void ExecuteToggleBookmarkCommand(object obj)
        {
            if (_view?.richTB != null)
            {
                var selection = _view?.richTB?.Selection;
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
                        BookmarkManagerHelper.AddBookmark(_prefixRun, runName);
                        HighlightManagerHelper.AddHighlight(_prefixRun, runName);
                    }
                }
                else
                {
                    // 获取选中内容的段落
                    Paragraph selectedParagraph = startPointer.Paragraph;
                    _view?.richTB?.Selection?.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Yellow);
                    _view?.richTB?.Selection?.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Green);
                    var run = startPointer.Parent as Run;
                    var runName = run.Name;
                    BookmarkManagerHelper.AddBookmark(_prefixRun, runName);
                    HighlightManagerHelper.AddHighlight(_prefixRun, runName);
                }
            }
        }

        private void ExecuteGoToBookmarkCommand(object obj)
        {
            var isNext = true;
            bool.TryParse(obj.ToString(), out isNext);

            if (_view?.richTB != null && _currentPointer != null)
            {
                var run = _currentPointer.Parent as Run;
                var runName = run.Name;

                var bookmarkPosition = BookmarkManagerHelper.GotoBookmark(_prefixRun, runName, isNext);
                if (bookmarkPosition != null)
                {
                    // 获取 RichTextBox 中的所有 TextElements
                    var targetRun = _view?.richTB?.Document?.Blocks
                                    .OfType<Paragraph>()
                                    .SelectMany(p => p.Inlines.OfType<Run>())
                                    .Where(run => run.Name.Equals(bookmarkPosition.RunName))
                                    .FirstOrDefault();

                    TextPointer selectionStart = targetRun?.ContentStart;
                    TextPointer selectionEnd = targetRun?.ContentEnd;

                    if (selectionStart == null || selectionEnd == null)
                        return;

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
            if (_view?.richTB != null)
            {
                // 获取选中文本起始和结束位置的物理区域
                Rect selectionStartRect = selectionStart.GetCharacterRect(LogicalDirection.Forward);
                Rect selectionEndRect = selectionEnd.GetCharacterRect(LogicalDirection.Backward);

                // 计算选中文本区域的高度
                double selectionHeight = selectionEndRect.Bottom - selectionStartRect.Top;

                // 获取 RichTextBox 内部的 ScrollViewer 控件
                ScrollViewer scrollViewer = GetScrollViewer(_view?.richTB);

                // 滚动到选中文本的位置，使其位于视窗的中间
                scrollViewer.ScrollToVerticalOffset(selectionStartRect.Top + scrollViewer.VerticalOffset - (selectionHeight / 2));
            }
        }

        private void ExecuteClearBookmarkCommand()
        {
            BookmarkManagerHelper.ClearBookmark();
            if (_view?.richTB != null)
            {
                LoadTextFile(_view?.richTB, _logFilePath);
            }
        }
    }
}
