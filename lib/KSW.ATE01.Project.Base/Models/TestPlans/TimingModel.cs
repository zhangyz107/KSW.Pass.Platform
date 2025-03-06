using KSW.ATE01.Project.Base.Enums.TestPlans;

namespace KSW.ATE01.Project.Base.Models.TestPlans
{
    /// <summary>
    /// 时钟模型
    /// </summary>
    [Serializable]
    public class TimingModel
    {
        /// <summary>
        /// 标识符
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 时钟名称
        /// </summary>
        public string TimingName { get; set; }

        /// <summary>
        /// 周期
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// 引脚Id
        /// </summary>
        public Guid PinId { get; set; }

        /// <summary>
        /// Pin名称
        /// </summary>
        public string PinName { get; set; }

        /// <summary>
        /// 限制名称
        /// </summary>
        public string PinSetup { get; set; }

        /// <summary>
        /// 波形格式
        /// </summary>
        public Timingformat Fmt { get; set; }

        /// <summary>
        /// DriveA
        /// </summary>
        public string DriveA { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DriveB { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DriveC { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DriveD { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public StrobeModeType StrobeMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int StrobeA { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int StrobeB { get; set; }

        /// <summary>
        /// 注解
        /// </summary>
        public string Comment { get; set; }
    }
}
