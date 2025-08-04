using KSW.ATE01.Drawing.Shmoo.Helper;
using KSW.ATE01.Drawing.Shmoo.IO.Enums.Shmoos;
using KSW.ATE01.Project.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Drawing.Shmoo.ViewModels
{
    public class MainViewModel : ViewModelExtension
    {
        public MainViewModel()
        {
            InitData();
        }

        private void InitData()
        {
            var xStart = 20.0;
            var xEnd = 30.0;
            //var xDelta = 0.5;
            var yStart = 3.0;
            var yEnd = 5.5;
            //var yDelta = 0.15;
            const int w = 30;
            const int h = 10;

            var rnd = new Random(DateTime.Now.Second);
            var data = new ShmooResultType[h, w];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    data[y, x] = (ShmooResultType)(rnd.Next(0, 5) * 20);
                }
            }

            ShmooChartHelper.DrawShmooChart(xStart, xEnd, yStart, yEnd, data, xAxisTitle: "频率（MHz）", yAxisTitle: "电压(V)");
            ShmooChartHelper.DrawShmooChart(xStart, xEnd, yStart, yEnd, data, xAxisTitle: "频率（MHz）", yAxisTitle: "电压(V)");
        }
    }
}
