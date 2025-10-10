using KSW.UI.WPF.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace KSW.UI.WPF.Controls
{
    public class WindowToastManager : WindowMessageManager, IToastManager
    {
        private readonly Window _hostWindow;
        private readonly StackPanel _toastContainer;
        private readonly Queue<ToastCard> _toasts = new();
        public int MaxToastCount { get; set; } = 5; // 默认最大数量

        static WindowToastManager()
        {
            //// 设置默认样式键
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowToastManager),
            //    new FrameworkPropertyMetadata(typeof(WindowToastManager)));
        }

        public WindowToastManager(UIElement element) : base(element)
        {
            //_hostWindow = hostWindow;

            // 创建一个透明 Grid 放在窗口上
            //_toastContainer = new StackPanel
            //{
            //    VerticalAlignment = VerticalAlignment.Top,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    Margin = new Thickness(10),
            //    IsHitTestVisible = false
            //};

            //if (_hostWindow.Content is Panel panel)
            //{
            //    panel.Children.Add(_toastContainer);
            //}
            //else
            //{
            //    var grid = new Grid();
            //    var originalContent = _hostWindow.Content;
            //    _hostWindow.Content = grid;
            //    if (originalContent is UIElement element)
            //        grid.Children.Add(element);
            //    grid.Children.Add(_toastContainer);
            //}
        }

        public void Show(IToast content)
        {
            Show(content, content.Type, content.Expiration,
                content.ShowIcon, content.ShowClose,
                content.OnClick, content.OnClose);
        }

        public override void Show(object content)
        {
            if (content is IToast toast)
            {
                Show(toast, toast.Type, toast.Expiration,
                    toast.ShowIcon, toast.ShowClose,
                    toast.OnClick, toast.OnClose);
            }
            else
            {
                Show(content, NotificationType.Information);
            }
        }

        public async void Show(
        object content,
        NotificationType type,
        TimeSpan? expiration = null,
        bool showIcon = true,
        bool showClose = true,
        Action? onClick = null,
        Action? onClose = null,
        string[]? classes = null)
        {
            var toastControl = new ToastCard
            {
                Content = content,
                NotificationType = type,
                ShowIcon = showIcon,
                ShowClose = showClose,
            };

            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    _toastContainer.Children.Insert(0, toastControl);
            //    _toasts.Enqueue(toastControl);

            //    if (_toasts.Count > MaxToastCount)
            //    {
            //        var oldToast = _toasts.Dequeue();
            //        _toastContainer.Children.Remove(oldToast);
            //    }

            //    // 动画
            //    var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(300)));
            //    toastControl.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            //    var timer = new DispatcherTimer
            //    {
            //        Interval = expiration ?? TimeSpan.FromMilliseconds(3000)
            //    };
            //    timer.Tick += (_, _) =>
            //    {
            //        timer.Stop();
            //        var fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(300)));
            //        fadeOut.Completed += (_, _) => _toastContainer.Children.Remove(toastControl);
            //        toastControl.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            //    };
            //    timer.Start();
            //});

            //toastControl.MessageClosed += ToastControl_MessageClosed;

            toastControl.MouseDown += (_, _) => { onClick?.Invoke(); };

            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, () =>
            {
                _items?.Add(toastControl);
                _toasts.Enqueue(toastControl);

                if (_toasts.Count > MaxToastCount)
                {
                    var oldToast = _toasts.Dequeue();
                    _items?.Remove(oldToast);
                }

                // 动画
                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(300)));
                toastControl.BeginAnimation(UIElement.OpacityProperty, fadeIn);

                var timer = new DispatcherTimer
                {
                    Interval = expiration ?? TimeSpan.FromMilliseconds(3000)
                };
                timer.Tick += (_, _) =>
                {
                    timer.Stop();
                    var fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(300)));
                    fadeOut.Completed += (_, _) => _items?.Remove(toastControl);
                    toastControl.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                };
                timer.Start();
            });

            //if (expiration == TimeSpan.Zero)
            //{
            //    return;
            //}

            //await Task.Delay(expiration ?? TimeSpan.FromSeconds(3));

            //toastControl.Close();
        }

        private void ToastControl_MessageClosed(object? sender, RoutedEventArgs e)
        {

        }
    }
}
