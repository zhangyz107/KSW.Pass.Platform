using KSW.ATE01.Drawing.Shmoo.IO.Enums.Shmoos;
using KSW.ATE01.Project.Base.Extensions;
using SciChart.Charting.Model.DataSeries.Heatmap2DArrayDataSeries;
using SciChart.Charting.Visuals.Axes.LabelProviders;
using SciChart.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;

namespace KSW.ATE01.Drawing.Shmoo.ViewModels.Dialogs
{
    public class HeatmapDialogViewModel : ViewModelExtension
    {
        private UniformHeatmapDataSeries<int, int, int> _chartData;
        private string _chartTitle;
        private string _xAxisTitle;
        private string _yAxisTitle;
        private bool _useLinearTextureFiltering;
        private ILabelProvider _xAxisLabelProvider;
        private ILabelProvider _yAxisLabelProvider;

        #region Properties
        public string Title => "Shmoo图";

        public UniformHeatmapDataSeries<int, int, int> ChartData
        {
            get => _chartData;
            set => SetProperty(ref _chartData, value);
        }

        public string XAxisTitle
        {
            get => _xAxisTitle;
            set => SetProperty(ref _xAxisTitle, value);
        }

        public string YAxisTitle
        {
            get => _yAxisTitle;
            set => SetProperty(ref _yAxisTitle, value);
        }

        public ILabelProvider XAxisLabelProvider
        {
            get => _xAxisLabelProvider;
            set => SetProperty(ref _xAxisLabelProvider, value);
        }

        public ILabelProvider YAxisLabelProvider
        {
            get => _yAxisLabelProvider;
            set => SetProperty(ref _yAxisLabelProvider, value);
        }

        public string ChartTitle
        {
            get => _chartTitle;
            set => SetProperty(ref _chartTitle, value);
        }

        /// <summary>
        /// 使用线性纹理过滤
        /// </summary>
        public bool UseLinearTextureFiltering
        {
            get => _useLinearTextureFiltering;
            set => SetProperty(ref _useLinearTextureFiltering, value);
        }

        #endregion

        public void SetShmooData(double xStart, double xEnd, double yStart, double yEnd, ShmooResultType[,] shmooValues, string chartTitle = null, string xAxisTitle = null, string yAxisTitle = null)
        {
            if (xEnd <= xStart)
                throw new ArgumentException($"{nameof(xEnd)}必须大于{nameof(xStart)}");

            if (yEnd <= yStart)
                throw new ArgumentException($"{nameof(yEnd)}必须大于{nameof(yStart)}");


            int rows = shmooValues.GetLength(0);
            int cols = shmooValues.GetLength(1);
            XAxisLabelProvider = new XAxisLabelProvider(xStart, xEnd, cols);
            YAxisLabelProvider = new YAxisLabelProvider(yStart, yEnd, rows);
            if (rows == 0)
                throw new ArgumentNullException($"{nameof(shmooValues)}一维长度为0");
            if (cols == 0)
                throw new ArgumentNullException($"{nameof(shmooValues)}二维长度为0");

            int[,] intArray = new int[rows, cols];

            Buffer.BlockCopy(shmooValues, 0, intArray, 0, shmooValues.Length * sizeof(int));

            ChartData = new UniformHeatmapDataSeries<int, int, int>(intArray, 0, 1, 0, 1);

            ChartTitle = string.IsNullOrEmpty(chartTitle) ? string.Empty : chartTitle;
            XAxisTitle = string.IsNullOrEmpty(xAxisTitle) ? string.Empty : xAxisTitle;
            YAxisTitle = string.IsNullOrEmpty(yAxisTitle) ? string.Empty : yAxisTitle;
        }
    }

    public class YAxisLabelProvider : LabelProviderBase
    {
        private double _min;
        private double _max;
        private double _delta;

        public YAxisLabelProvider(double min, double max, int count)
        {
            _min = min;
            _max = max;
            if (count == 0)
                count = 5;

            _delta = Math.Abs(max - min) / count;
        }

        public override string FormatCursorLabel(IComparable dataValue)
        {
            return FormatLabel(dataValue);
        }

        public override string FormatLabel(IComparable dataValue)
        {
            try
            {
                var sacle = Convert.ToInt32(dataValue);
                var value = _min + _delta * sacle;
                return $"{value:F2}";
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public class XAxisLabelProvider : LabelProviderBase
    {
        private double _min;
        private double _max;
        private double _delta;

        public XAxisLabelProvider(double min, double max, int count)
        {
            _min = min;
            _max = max;
            if (count == 0)
                count = 10;

            _delta = Math.Abs(max - min) / (count / 2);
        }

        public override string FormatCursorLabel(IComparable dataValue)
        {
            return FormatLabel(dataValue);

        }

        public override string FormatLabel(IComparable dataValue)
        {
            try
            {
                var sacle = Convert.ToInt32(dataValue) / 2;
                var value = _min + _delta * sacle;

                return $"{value:F2}";
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
