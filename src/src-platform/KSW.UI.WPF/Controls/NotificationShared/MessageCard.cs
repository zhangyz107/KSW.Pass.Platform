using KSW.UI.WPF.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace KSW.UI.WPF.Controls
{
    public abstract class MessageCard : ContentControl
    {
        private readonly Storyboard _showStoryboard;
        private readonly Storyboard _hideStoryboard;

        /// <summary>
        /// Determines if the message is already closing.
        /// </summary>
        public bool IsClosing
        {
            get { return (bool)GetValue(IsClosingProperty); }
            set { SetValue(IsClosingProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="IsClosing"/> property.
        /// </summary>
        public static readonly DependencyProperty IsClosingProperty =
            DependencyProperty.Register(nameof(IsClosing), typeof(bool), typeof(MessageCard), new PropertyMetadata(false));

        public bool IsClosed
        {
            get { return (bool)GetValue(IsClosedProperty); }
            set { SetValue(IsClosedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsClosed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsClosedProperty =
            DependencyProperty.Register(nameof(IsClosed), typeof(bool), typeof(MessageCard));

        /// <summary>
        /// Gets or sets the type of the message
        /// </summary>
        public NotificationType NotificationType
        {
            get => (NotificationType)GetValue(NotificationTypeProperty);
            set => SetValue(NotificationTypeProperty, value);
        }

        /// <summary>
        /// Defines the <see cref="NotificationType" /> property
        /// </summary>
        public static readonly DependencyProperty NotificationTypeProperty =
            DependencyProperty.Register(nameof(NotificationType), typeof(NotificationType), typeof(MessageCard));

        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.Register(nameof(ShowIcon), typeof(bool), typeof(MessageCard), new PropertyMetadata(true));  // 默认值 true

        public bool ShowClose
        {
            get => (bool)GetValue(ShowCloseProperty);
            set => SetValue(ShowCloseProperty, value);
        }

        public static readonly DependencyProperty ShowCloseProperty =
            DependencyProperty.Register(nameof(ShowClose), typeof(bool), typeof(MessageCard), new PropertyMetadata(true));  // 默认值 true

        /// <summary>
        /// Defines the <see cref="MessageClosed"/> event.
        /// </summary>
        public static readonly RoutedEvent MessageClosedEvent =
            EventManager.RegisterRoutedEvent(nameof(MessageClosed), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MessageCard));

        /// <summary>
        /// Raised when the <see cref="MessageCard"/> has closed.
        /// </summary>
        public event EventHandler<RoutedEventArgs>? MessageClosed
        {
            add => AddHandler(MessageClosedEvent, value);
            remove => RemoveHandler(MessageClosedEvent, value);
        }

        public static bool GetCloseOnClick(Button obj)
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));
            return (bool)obj.GetValue(CloseOnClickProperty);
        }

        public static void SetCloseOnClick(Button obj, bool value)
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));
            obj.SetValue(CloseOnClickProperty, value);
        }

        /// <summary>
        /// Defines the CloseOnClick property.
        /// </summary>
        public static readonly DependencyProperty CloseOnClickProperty =
            DependencyProperty.RegisterAttached("CloseOnClick", typeof(bool), typeof(MessageCard), new PropertyMetadata(false, OnCloseOnClickPropertyChanged));

        protected MessageCard()
        {
            Margin = new Thickness(0, 5, 0, 5);
            HorizontalAlignment = HorizontalAlignment.Center;

            // 显示动画
            _showStoryboard = new Storyboard();
            var showAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTargetProperty(showAnimation, new PropertyPath(OpacityProperty));
            _showStoryboard.Children.Add(showAnimation);

            // 隐藏动画
            _hideStoryboard = new Storyboard();
            var hideAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTargetProperty(hideAnimation, new PropertyPath(OpacityProperty));
            _hideStoryboard.Children.Add(hideAnimation);

            //_hideStoryboard.Completed += (s, e) => OnHideCompleted();

            Opacity = 0;
        }

        private static void OnCloseOnClickPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (Button)d;
            var value = (bool)e.NewValue;

            if ((bool)e.OldValue)
            {
                button.Click -= Button_Click;
            }

            if (value)
            {
                button.Click += Button_Click;
            }
        }

        private static void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as DependencyObject;
            var message = FindVisualAncestor<MessageCard>(btn);
            message?.Close();
        }

        /// <summary>
        /// Closes the <see cref="MessageCard"/>.
        /// </summary>
        public void Close()
        {
            if (IsClosing)
            {
                return;
            }

            IsClosing = true;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ContentProperty && e.NewValue is IMessage message)
            {
                SetValue(NotificationTypeProperty, message.Type);
            }

            if (e.Property == IsClosedProperty)
            {
                if (!IsClosing && !IsClosed)
                {
                    return;
                }

                RaiseEvent(new RoutedEventArgs(MessageClosedEvent));
            }
        }

        // 辅助方法：查找视觉树上的祖先元素
        public static T FindVisualAncestor<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T ancestor)
                    return ancestor;
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }
    }
}
