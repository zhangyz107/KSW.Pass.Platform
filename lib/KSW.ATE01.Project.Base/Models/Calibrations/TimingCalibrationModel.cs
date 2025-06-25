using KSW.ATE01.Project.Base.Enums.Calibrations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace KSW.ATE01.Project.Base.Models.Calibrations
{
    /// <summary>
    /// 时钟校准数据模型
    /// </summary>
    public class TimingCalibrationModel
    {
        /// <summary>
        /// 校准类型
        /// </summary>
        [JsonPropertyName("calibrationType")]
        public string CalibrationTypeString { get; set; }
        /// <summary>
        /// 校准类型
        /// </summary>
        [JsonIgnore]
        public TimingCalibrationType CalibrationType => GetCalibrationType();

        /// <summary>
        /// 插槽
        /// </summary>
        [JsonPropertyName("slot")]
        public string Slot { get; set; }

        /// <summary>
        /// 插槽编号
        /// </summary>
        [JsonIgnore]
        public int SlotNum => GetSlotNum();

        /// <summary>
        /// 值列表
        /// </summary>
        [JsonPropertyName("value")]
        public List<string> ValueStrings { get; set; }

        // 解析后的数值列表
        [JsonIgnore]
        public List<double> Values => ValueStrings.FirstOrDefault()?
            .Split(',')
            .Select(s => ParseDouble(s.Trim()))
            .ToList() ?? new List<double>();

        private TimingCalibrationType GetCalibrationType()
        {
            if (Enum.TryParse(CalibrationTypeString, out TimingCalibrationType type))
                return type;

            return TimingCalibrationType.None;
        }


        private int GetSlotNum()
        {
            if (string.IsNullOrEmpty(Slot))
                return 0;
            else if (int.TryParse(Slot, out int slotNum))
                return slotNum;
            else
                return 0;
        }

        private double ParseDouble(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            return double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double result)
                ? result : 0;
        }
    }
}
