using KSW.Application;
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
        void SetCompilerPath(string patternFilePath, string testPlanFilePath, string testPlanSheetName, string outputBinFilePath, bool saveComment = true);

        int CompilePattern(string tempFolder = "");
    }
}
