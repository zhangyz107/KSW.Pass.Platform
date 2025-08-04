using KSW.ATE01.Drawing.Shmoo.IO.Enums.Shmoos;
using KSW.ATE01.Drawing.Shmoo.ViewModels.Dialogs;
using KSW.ATE01.Drawing.Shmoo.Views.Dialogs;

namespace KSW.ATE01.Drawing.Shmoo.Helper
{
    /// <summary>
    /// Shmoo图帮助类
    /// </summary>
    public class ShmooChartHelper
    {
        /// <summary>
        /// 使用线性纹理过滤
        /// </summary>
        public static bool LinearTextureFiltering { get; set; } = false;

        public static void DrawShmooChartDialog(double xStart, double xEnd, double yStart, double yEnd, ShmooResultType[,] shmooValues, string chartTitle = null, string xAxisTitle = null, string yAxisTitle = null)
        {
            var shmooChart = new HeatmapDialogView();
            if (shmooChart.DataContext is HeatmapDialogViewModel viewModel)
            {
                viewModel.UseLinearTextureFiltering = LinearTextureFiltering;
                viewModel.SetShmooData(xStart, xEnd, yStart, yEnd, shmooValues, chartTitle, xAxisTitle, yAxisTitle);
            }
            shmooChart.ShowDialog();
        }

        public static void DrawShmooChart(double xStart, double xEnd, double yStart, double yEnd, ShmooResultType[,] shmooValues, string chartTitle = null, string xAxisTitle = null, string yAxisTitle = null)
        {
            var shmooChart = new HeatmapDialogView();
            if (shmooChart.DataContext is HeatmapDialogViewModel viewModel)
            {
                viewModel.UseLinearTextureFiltering = LinearTextureFiltering;
                viewModel.SetShmooData(xStart, xEnd, yStart, yEnd, shmooValues, chartTitle, xAxisTitle, yAxisTitle);
            }
            shmooChart.Show();
        }
    }
}
