using KSW.UI.WPF.Enums;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace KSW.UI.WPF.Controls
{
    /// <summary>
    /// Marquee.xaml 的交互逻辑（走马灯控件）
    /// </summary>
    public partial class Marquee : ContentControl
    {
        private ContentPresenter _presenter;
        private System.Timers.Timer? _timer;

        /// <summary>
        /// 正在运行
        /// </summary>
        public bool IsRunning
        {
            get { return (bool)GetValue(IsRunningProperty); }
            set
            {
                SetValue(IsRunningProperty, value);
                OnIsRunningChanged(value);
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRunningProperty =
            DependencyProperty.Register(nameof(IsRunning), typeof(bool), typeof(Marquee), new PropertyMetadata(true));

        /// <summary>
        /// 方向
        /// </summary>
        public Direction Direction
        {
            get { return (Direction)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register(nameof(Direction), typeof(Direction), typeof(Marquee));

        /// <summary>
        /// 滚动速度
        /// </summary>
        public double Speed
        {
            get { return (double)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register(nameof(Speed), typeof(double), typeof(Marquee), new PropertyMetadata(60.0), OnCoerceSpeed);

        private static bool OnCoerceSpeed(object value)
        {
            if (value is not double val || val < 0)
                return false;

            return true;
        }

        /// <summary>
        /// 边框厚度
        /// </summary>
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(nameof(BorderThickness), typeof(Thickness), typeof(Marquee));


        /// <summary>
        /// 圆角半径
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadiusProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(Marquee));



        static Marquee()
        {
            ClipToBoundsProperty.OverrideMetadata(typeof(Marquee), new FrameworkPropertyMetadata(true));
        }

        public Marquee()
        {
            InitializeComponent();

            _timer = new System.Timers.Timer();
            _timer.Interval = 1000 / 60.0;
            this.DataContext = this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _presenter = GetTemplateChild("PAPT_ContentPresenter") as ContentPresenter;
        }

        private void OnIsRunningChanged(bool value)
        {
            if (value)
            {
                _timer?.Start();
            }
            else
            {
                _timer?.Stop();
            }
        }

        private LayoutValues GetLayoutValues()
        {
            var transform = _presenter?.TransformToVisual(this);
            var presenterPosition = transform?.Transform(new Point(0, 0)) ?? new Point();
            return new LayoutValues()
            {
                Bounds = RenderSize,
                PresenterSize = _presenter?.RenderSize ?? new Size(),
                Left = _presenter is null ? 0 : Canvas.GetLeft(_presenter),
                Top = _presenter is null ? 0 : Canvas.GetTop(_presenter),
                Diff = IsRunning ? Speed / 60.0 : 0,
                HorizontalAlignment = HorizontalContentAlignment,
                VerticalAlignment = VerticalContentAlignment,
                Direction = Direction,
            };
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded; // 避免重复订阅

            // 查找Presenter（假设是ContentPresenter）
            if (_presenter != null)
            {
                _presenter.SizeChanged += OnPresenterSizeChanged;
            }

            _timer?.Stop();
            _timer?.Dispose();
            _timer = new System.Timers.Timer();
            _timer.Interval = 1000 / 60.0;
            _timer.Elapsed += TimerOnTick;

            if (IsRunning)
            {
                _timer.Start();
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Elapsed -= TimerOnTick;
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            // 查找并清理Presenter
            if (_presenter != null)
            {
                _presenter.SizeChanged -= OnPresenterSizeChanged;
            }
        }

        private void OnPresenterSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidatePresenterPosition();
        }

        private void TimerOnTick(object? sender, ElapsedEventArgs e)
        {
            //var presenter = FindName(_contentPresenter) as ContentPresenter;

            //if (presenter is null) return;
            //var layoutValues = GetLayoutValues();
            //var location = UpdateLocation(layoutValues);
            //if (location is null) return;
            //Application.Current.Dispatcher.BeginInvoke(() =>
            //{
            //    Canvas.SetTop(presenter, location.Value.top);
            //    Canvas.SetLeft(presenter, location.Value.left);
            //}, DispatcherPriority.Render);
            if (Application.Current is null)
                return;

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                InvalidatePresenterPosition();
            }, DispatcherPriority.Normal);
        }

        private void InvalidatePresenterPosition()
        {
            if (_presenter is null) return;
            var layoutValues = GetLayoutValues();
            var location = UpdateLocation(layoutValues);
            if (location is null) return;
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                Canvas.SetTop(_presenter, location.Value.top);
                Canvas.SetLeft(_presenter, location.Value.left);
            }, DispatcherPriority.Render);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var result = base.MeasureOverride(constraint);
            if (_presenter is null) return result;
            var size = _presenter.DesiredSize;

            // 处理宽度
            if (double.IsInfinity(result.Width) || result.Width == 0)
            {
                result = new Size(size.Width, result.Height);
            }

            // 处理高度
            if (double.IsInfinity(result.Height) || result.Height == 0)
            {
                result = new Size(result.Width, size.Height);
            }

            return result;
        }

        private (double top, double left)? UpdateLocation(LayoutValues values)
        {
            var horizontalOffset = values.Direction switch
            {
                Direction.Up or Direction.Down => GetHorizontalOffset(values.Bounds, values.PresenterSize, values.HorizontalAlignment),
                Direction.Left or Direction.Right => values.Left,
                _ => throw new NotImplementedException(),
            };
            var verticalOffset = values.Direction switch
            {
                Direction.Up or Direction.Down => values.Top,
                Direction.Left or Direction.Right => GetVerticalOffset(values.Bounds, values.PresenterSize, values.VerticalAlignment),
                _ => throw new NotImplementedException(),
            };
            if (horizontalOffset is double.NaN) horizontalOffset = 0.0;
            if (verticalOffset is double.NaN) verticalOffset = 0.0;
            var speed = values.Diff;
            var diff = values.Direction switch
            {
                Direction.Up => -speed,
                Direction.Down => speed,
                Direction.Left => -speed,
                Direction.Right => speed,
                _ => 0
            };
            switch (values.Direction)
            {
                case Direction.Up:
                case Direction.Down:
                    verticalOffset += diff;
                    break;
                case Direction.Left:
                case Direction.Right:
                    horizontalOffset += diff;
                    break;
            }
            switch (values.Direction)
            {
                case Direction.Down:
                    if (verticalOffset > values.Bounds.Height) verticalOffset = -values.PresenterSize.Height;
                    break;
                case Direction.Up:
                    if (verticalOffset < -values.PresenterSize.Height) verticalOffset = values.Bounds.Height;
                    break;
                case Direction.Right:
                    if (horizontalOffset > values.Bounds.Width) horizontalOffset = -values.PresenterSize.Width;
                    break;
                case Direction.Left:
                    if (horizontalOffset < -values.PresenterSize.Width) horizontalOffset = values.Bounds.Width;
                    break;
            }
            verticalOffset = MathHelpers.SafeClamp(verticalOffset, -values.PresenterSize.Height, values.Bounds.Height);
            horizontalOffset = MathHelpers.SafeClamp(horizontalOffset, -values.PresenterSize.Width, values.Bounds.Width);
            return (verticalOffset, horizontalOffset);
        }

        private double GetHorizontalOffset(Size bounds, Size presenterBounds, HorizontalAlignment horizontalAlignment)
        {
            return horizontalAlignment switch
            {
                HorizontalAlignment.Left => 0,
                HorizontalAlignment.Center => (bounds.Width - presenterBounds.Width) / 2,
                HorizontalAlignment.Right => bounds.Width - presenterBounds.Width,
                _ => 0
            };
        }

        private double GetVerticalOffset(Size bounds, Size presenterBounds, VerticalAlignment verticalAlignment)
        {
            return verticalAlignment switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Center => (bounds.Height - presenterBounds.Height) / 2,
                VerticalAlignment.Bottom => bounds.Height - presenterBounds.Height,
                _ => 0
            };
        }
    }
}
