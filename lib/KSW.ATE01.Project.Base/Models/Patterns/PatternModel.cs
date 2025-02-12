using KSW.ATE01.Project.Base.Enums.Patterns;

namespace KSW.ATE01.Project.Base.Models.Patterns
{
    /// <summary>
    /// 向量模型
    /// </summary>
    public class PatternModel
    {
        /// <summary>
        /// 向量文件名
        /// </summary>
        public string PatternFileName { get; set; }

        /// <summary>
        /// 时钟设置
        /// </summary>
        public List<string> TimingSets { get; set; } = new List<string>();

        /// <summary>
        /// 设备名
        /// </summary>
        public string InstrumentName { get; set; }

        /// <summary>
        /// 设备信息
        /// </summary>
        public InstrumentModel InstrumentInfo { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        public ModuleType ModuleType { get; set; }

        /// <summary>
        /// 模式向量集合
        /// </summary>
        public List<PatternVectorModel> PatternVectors { get; set; } = new List<PatternVectorModel>();
    }
}
