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
        private ShmooViewModel _shmooViewModel = new ShmooViewModel();

        public ShmooViewModel ShmooViewModel
        {
            get => _shmooViewModel;
            set => SetProperty(ref _shmooViewModel, value);
        }

        public MainViewModel()
        {
            InitData();
        }

        private void InitData()
        {
            var xStart = 20.0;
            var xEnd = 30.0;
            var xDelta = 0.5;
            var yStart = 3.0;
            var yEnd = 5.5;
            var yDelta = 0.15;

            var xDistence = xEnd - xStart;
            var rowLength = (int)Math.Round(xDistence / xDelta, 0);
            var yDistence = yEnd - yStart;
            var colLength = (int)Math.Round(yDistence / yDelta, 0);

            var data = new bool?[rowLength, colLength];

            for (var i = 0; i < rowLength; i++)
                for (var j = 0; j < colLength; j++)
                {
                    if (i < rowLength / 2 || j < colLength / 2)
                        data[i, j] = false;
                    else if (i == rowLength / 2 || j == colLength / 2)
                    {
                        data[i, j] = null;
                    }
                    else
                        data[i, j] = true;
                }

            ShmooViewModel.SetShmooData(20.0, 30.0, 0.5, 3, 5.5, 0.15, data, "X", "Y");
        }
    }
}
