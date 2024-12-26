using KSW.Application;
using KSW.ATE01.Application.Models.RealTimeTxt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.BLLs.Abstractions.RealTimeTxt
{
    /// <summary>
    /// 文件配置逻辑层接口
    /// </summary>
    public interface IConfigureFileBLL : IService
    {
        /// <summary>
        /// 从缓存中获取文件配置
        /// </summary>
        ConfigureFileModel GetConfigureFile();

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="configureFile"></param>
        void SaveConfigureFile(ConfigureFileModel configureFile);

        /// <summary>
        /// 保存文件配置到磁盘
        /// </summary>
        void SaveConfigureFileToDisk(ConfigureFileModel configureFile);
    }
}
