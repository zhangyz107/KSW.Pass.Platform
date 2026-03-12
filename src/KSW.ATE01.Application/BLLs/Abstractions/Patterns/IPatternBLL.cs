using KSW.Application;
using KSW.ATE01.Application.Models.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.BLLs.Abstractions.Patterns
{
    /// <summary>
    /// 向量业务逻辑接口
    /// </summary>
    public interface IPatternBLL : IService
    {

        /// <summary>
        /// 从文件或文件夹中获取向量
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Task<List<PatternModel>> GetPatternsByFilesAsync(string path, bool isDir = false);

        /// <summary>
        /// 保存向量
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public Task<bool> SavePattern(PatternModel pattern, string savePath);

        /// <summary>
        /// 编译向量
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Task<bool> CompileAsync(PatternModel pattern, string filePath);
    }
}
