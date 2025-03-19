using KSW.ATE01.Project.Base.Extensions;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Model.DataSeries.Heatmap2DArrayDataSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Drawing.Shmoo.ViewModels
{
    public class ShmooViewModel : ViewModelExtension
    {
        private double _boxWidth;
        private string _xAxisTitle;
        private string _yAxisTitle;
        private IDataSeries _greenChartData;
        private IDataSeries _redChartData;
        private IDataSeries _whiteChartData;

        public IDataSeries GreenChartData
        {
            get => _greenChartData;
            set => SetProperty(ref _greenChartData, value);
        }

        public double BoxWidth
        {
            get => _boxWidth;
            set => SetProperty(ref _boxWidth, value);
        }

        public IDataSeries RedChartData
        {
            get => _redChartData;
            set => SetProperty(ref _redChartData, value);
        }

        public IDataSeries WhiteChartData
        {
            get => _whiteChartData;
            set => SetProperty(ref _whiteChartData, value);
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

        public void SetShmooData(double xStart, double xEnd, double xDelta, double yStart, double yEnd, double yDelta, bool?[,] shmooValue, string xAxisTitle = null, string yAxisTitle = null)
        {
            if (xEnd <= xStart)
                throw new ArgumentException($"{nameof(xEnd)}必须大于{nameof(xStart)}");

            if (yEnd <= yStart)
                throw new ArgumentException($"{nameof(yEnd)}必须大于{nameof(yStart)}");

            var row = shmooValue.GetLength(0);
            var col = shmooValue.GetLength(1);

            BoxWidth = xDelta * 2;
            var greenBoxDatas = new BoxPlotDataSeries<double, double>();
            var redBoxDatas = new BoxPlotDataSeries<double, double>();
            var whiteBoxDatas = new BoxPlotDataSeries<double, double>();

            for (var j = 0; j < col; j++)
            {
                var startX = j * xDelta + xStart;
                var endX = (j + 1) * xDelta + xStart;

                for (var i = 0; i < row; i++)
                {
                    var startY = i * yDelta + yStart;
                    var endY = (i + 1) * yDelta + yStart;
                    switch (shmooValue[i, j])
                    {
                        case true:
                            greenBoxDatas.Append(startX, startY, startY, startY, endY, endY);
                            break;
                        case false:
                            redBoxDatas.Append(startX, startY, startY, startY, endY, endY);
                            break;
                        default:
                            whiteBoxDatas.Append(startX, startY, startY, startY, endY, endY);
                            break;
                    }
                }
            }

            GreenChartData = greenBoxDatas;
            RedChartData = redBoxDatas;
            WhiteChartData = whiteBoxDatas;
            XAxisTitle = string.IsNullOrEmpty(xAxisTitle) ? string.Empty : xAxisTitle;
            YAxisTitle = string.IsNullOrEmpty(yAxisTitle) ? string.Empty : yAxisTitle;
        }
    }
}
