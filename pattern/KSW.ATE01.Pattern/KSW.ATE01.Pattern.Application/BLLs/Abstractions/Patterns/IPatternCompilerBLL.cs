using KSW.Application;
using KSW.ATE01.Pattern.Application.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.BLLs.Abstractions.Patterns
{
    /// <summary>
    /// 向量编译业务逻辑服务接口
    /// </summary>
    public interface IPatternCompilerBLL : IService
    {
        /// <summary>
        /// 解析向量文件
        /// </summary>
        PatternModel AnalysisPattern(string patternFilePath);

        /// <summary>
        /// 导出atp文件
        /// </summary>
        void ExportPattern(PatternModel patternModel);
    }
}
