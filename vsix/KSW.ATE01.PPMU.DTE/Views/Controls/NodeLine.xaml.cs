using KSW.ATE01.PPMU.Domain.Core.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KSW.ATE01.PPMU.Start.Views.Controls
{
    /// <summary>
    /// NodeLine.xaml 的交互逻辑
    /// </summary>
    public partial class NodeLine : UserControl
    {
        internal int waveCount = 4; //波浪数量
        public PointCollection WavePoints { get; set; }

        public NodeLine()
        {
            InitializeComponent();
            this.DataContext = this;
            InitShape();
        }

        private void InitShape()
        {
            Line.Visibility = Visibility.Visible;
            Wave.Visibility = Visibility.Collapsed;
        }

        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        // Using a DependencyProperty as the backing store for X1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(NodeLine), new PropertyMetadata(0.0, GenerateWavePath));

        private static void GenerateWavePath(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NodeLine nodeLine)
            {
                var circleDiameter = nodeLine.Diameter;

                var wavePoints = new PointCollection();
                wavePoints.Add(new Point(nodeLine.X1, nodeLine.Y1 + circleDiameter / 2));
                var waveStartX = Math.Abs(nodeLine.X2 - nodeLine.X1) / 4 + nodeLine.X1;
                var waveEndX = nodeLine.X2 - Math.Abs(nodeLine.X2 - nodeLine.X1) / 4;
                var waveLength = Math.Sqrt((nodeLine.X2 - nodeLine.X1) * (nodeLine.X2 - nodeLine.X1) / 4 + (nodeLine.Y2 - nodeLine.Y1) * (nodeLine.Y2 - nodeLine.Y1) / 4);
                waveLength /= 4;
                var middleY = circleDiameter / 2 + Math.Abs(nodeLine.Y2 - nodeLine.Y1) / 2 + (nodeLine.Y1 <= nodeLine.Y2 ? nodeLine.Y1 : nodeLine.Y2);
                wavePoints.Add(new Point(waveStartX, nodeLine.Y1 + circleDiameter / 2));

                var lastStartX = waveStartX;
                for (int i = 1; i <= nodeLine.waveCount; i++)
                {
                    double x1 = lastStartX + waveLength / 4;
                    double y1 = middleY - circleDiameter;
                    var w1 = new Point(x1, y1);

                    double x2 = lastStartX + waveLength / 2;
                    double y2 = middleY;
                    var w2 = new Point(x2, y2);

                    double x3 = lastStartX + waveLength / 4 * 3;
                    double y3 = middleY + circleDiameter;
                    var w3 = new Point(x3, y3);

                    double x4 = waveLength + lastStartX;
                    double y4 = middleY;
                    var w4 = new Point(x4, y4);

                    wavePoints.Add(w1);
                    wavePoints.Add(w2);
                    wavePoints.Add(w3);
                    wavePoints.Add(w4);

                    lastStartX = x4;
                }
                wavePoints.Add(new Point(nodeLine.X2, nodeLine.Y1 + circleDiameter / 2));
                nodeLine.WavePoints = wavePoints;

            }
        }

        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        // Using a DependencyProperty as the backing store for X2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(NodeLine), new PropertyMetadata(0.0, GenerateWavePath));



        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        // Using a DependencyProperty as the backing store for Y1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(NodeLine), new PropertyMetadata(0.0, GenerateWavePath));



        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        // Using a DependencyProperty as the backing store for Y2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(NodeLine), new PropertyMetadata(0.0, GenerateWavePath));



        public double Diameter
        {
            get { return (double)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Diameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(double), typeof(NodeLine), new PropertyMetadata(0.0, GenerateWavePath));



        public Visibility StartPointVisibility
        {
            get { return (Visibility)GetValue(StartPointVisibilityProperty); }
            set { SetValue(StartPointVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartPointVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartPointVisibilityProperty =
            DependencyProperty.Register("StartPointVisibility", typeof(Visibility), typeof(NodeLine), new PropertyMetadata(Visibility.Visible));



        public Visibility EndPointVisibility
        {
            get { return (Visibility)GetValue(EndPointVisibilityProperty); }
            set { SetValue(EndPointVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndPointVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndPointVisibilityProperty =
            DependencyProperty.Register("EndPointVisibility", typeof(Visibility), typeof(NodeLine), new PropertyMetadata(Visibility.Visible));



        public NodeLineShape LineShape
        {
            get { return (NodeLineShape)GetValue(LineShapeProperty); }
            set { SetValue(LineShapeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineShapeProperty =
            DependencyProperty.Register("LineShapeProperty", typeof(NodeLineShape), typeof(NodeLine), new PropertyMetadata(NodeLineShape.Line, LineShapeChanged));

        private static void LineShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NodeLine nodeLine && e.NewValue is NodeLineShape shape)
            {
                switch (shape)
                {
                    case NodeLineShape.Line:
                        nodeLine.Line.Visibility = Visibility.Visible;
                        nodeLine.Wave.Visibility = Visibility.Collapsed;
                        break;
                    case NodeLineShape.WaveLine:
                        nodeLine.Line.Visibility = Visibility.Collapsed;
                        nodeLine.Wave.Visibility = Visibility.Visible;

                        GenerateWavePath(d, e);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
