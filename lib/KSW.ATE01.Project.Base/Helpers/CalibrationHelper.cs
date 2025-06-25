using KSW.ATE01.Project.Base.Models.Calibrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Helpers
{
    public class CalibrationHelper
    {
        public static string CalDirectory = "C:\\ATE01\\Calibration";

        /// <summary>
        /// 获取Timing校准数据
        /// </summary>
        /// <param name="calFile"></param>
        /// <returns></returns>
        public static List<TimingCalibrationModel> GetTimingCalibrations(string calFile)
        {
            var result = new List<TimingCalibrationModel>();
            if (!File.Exists(calFile))
                return result;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,  // 允许结尾逗号
                ReadCommentHandling = JsonCommentHandling.Skip,  // 跳过注释
            };

            try
            {
                var data = JsonSerializer.Deserialize<List<TimingCalibrationModel>>(File.ReadAllText(calFile), options);
                result = data ?? result;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError($"JSON解析错误: {ex.Message}");
            }

            return result;
        }
    }
}
