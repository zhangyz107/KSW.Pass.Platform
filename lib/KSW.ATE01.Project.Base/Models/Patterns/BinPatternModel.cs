using KSW.ATE01.Project.Base.Enums.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Patterns
{
    /// <summary>
    /// 二进制向量模式
    /// </summary>
    public class BinPatternModel
    {
        /// <summary>
        /// 向量文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 向量名称
        /// </summary>
        public string VectorName { get; set; }

        /// <summary>
        /// 数据起始地址
        /// </summary>
        public long DataStartAddress { get; set; }

        /// <summary>
        /// 数据结束地址
        /// </summary>
        public long DataEndAddress { get; set; }

        /// <summary>
        /// 引脚数据长度
        /// </summary>
        public int PinDataLength { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        public ModuleType ModuleType { get; set; }

        /// <summary>
        /// 引脚打包模型
        /// </summary>
        public List<PinPackModel> PinPacks { get; set; }
    }
}
