using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KSW.ATE01.Application.Models.Patterns
{
    /// <summary>
    /// 向量信息模型
    /// </summary>
    public class PatternModel : DtoBase
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 向量名
        /// </summary>
        public string VectorName { get; set; }

        /// <summary>
        /// 时钟设置
        /// </summary>
        public List<string> TimingSets { get; set; } = new List<string>();

        /// <summary>
        /// 设备名
        /// </summary>
        public string InstrumentName { get; set; }

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
