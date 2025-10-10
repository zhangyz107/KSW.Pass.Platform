using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace KSW.UI.WPF.Controls
{
    [TemplatePart(Name = PART_Items, Type = typeof(Panel))]
    public abstract class WindowMessageManager : Adorner
    {
        private readonly VisualCollection _visuals;
        private readonly StackPanel _container;

        public StackPanel Container => _container;

        public const string PART_Items = "PART_Items";

        protected IList? _items;

        /// <summary>
        /// Defines the <see cref="MaxItems"/> property.
        /// </summary>
        public static readonly DependencyProperty MaxItemsProperty =
            DependencyProperty.Register(nameof(MaxItems), typeof(int), typeof(WindowMessageManager),
                new PropertyMetadata(5));

        /// <summary>
        /// Defines the maximum number of messages visible at once.
        /// </summary>
        public int MaxItems
        {
            get => (int)GetValue(MaxItemsProperty);
            set => SetValue(MaxItemsProperty, value);
        }

        public WindowMessageManager(UIElement element) : base(element)
        {
            _container = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 20, 0, 0),
                IsHitTestVisible = false
            };

            AddVisualChild(_container);
            //_visuals.Add(_container);
            _items = _container?.Children;

        }

        public abstract void Show(object content);

        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => _container;

        protected override Size MeasureOverride(Size constraint)
        {
            _container.Measure(constraint);
            return _container.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.AdornedElement.RenderSize;
            var center = size.Width / 2;
            // 水平居中，顶部偏移10
            double x = Math.Abs((finalSize.Width - size.Width) / 2);
            double y = 10; // 顶部偏移
            _container.Arrange(new Rect(new Point(x, y), _container.DesiredSize));
            return finalSize;
        }

        ///// <summary>
        ///// Installs the <see cref="WindowMessageManager"/> within the <see cref="AdornerLayer"/>
        ///// </summary>
        //protected void InstallFromWindow(Window window)
        //{
        //    window.Loaded += WindowOnLoaded;

        //    // 尝试立即安装
        //    var adornerDecorator = FindVisualChild<AdornerDecorator>(window);
        //    if (adornerDecorator?.AdornerLayer is AdornerLayer adornerLayer)
        //    {
        //        adornerLayer.Add(this);
        //        //AdornerLayer.SetAdornedElement(this, adornerLayer);
        //    }
        //}

        //public virtual void Uninstall()
        //{
        //    if (Parent is AdornerLayer adornerLayer)
        //    {
        //        adornerLayer.Remove(this);
        //        //AdornerLayer.SetAdornedElement(this, null);
        //    }
        //}

        //protected void WindowOnLoaded(object sender, RoutedEventArgs e)
        //{
        //    if (Parent is AdornerLayer adornerLayer)
        //    {
        //        adornerLayer.Remove(this);
        //        //AdornerLayer.SetAdornedElement(this, null);
        //    }

        //    // 重新安装消息管理器
        //    var window = (Window)sender;
        //    window.Loaded -= WindowOnLoaded;
        //    InstallFromWindow(window);
        //}

        /// <summary>
        /// 在可视化树中查找指定类型的子元素
        /// </summary>
        public static T? FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T result)
                    return result;

                var childResult = FindVisualChild<T>(child);
                if (childResult != null)
                    return childResult;
            }
            return null;
        }
    }
}
