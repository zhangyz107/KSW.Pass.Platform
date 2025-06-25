using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Calibrations
{
    /// <summary>
    /// 所有校准数据模型
    /// </summary>
    public class CalibrationsDataModel
    {
        /// <summary>
        /// 时钟校准数据集合
        /// </summary>
        public List<TimingCalibrationModel> Timing { get; set; } = new List<TimingCalibrationModel>();
    }
}
