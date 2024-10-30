using KSW.ATE01.Extension.Shared.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KSW.ATE01.Extension.Shared.Helpers.Projects
{
    /// <summary>
    /// 项目帮助类
    /// </summary>
    public class ProjectHelper
    {
        public static string ProjectExt { get; } = ".atecfg";

        /// <summary>
        /// 加载项目信息
        /// </summary>
        /// <returns></returns>
        public static ProjectInfo LoadProjectInfo(string configPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
                {
                    var projectInfo = XmlHelper.DeserializeFromXml<ProjectInfo>(configPath);
                    return projectInfo;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取项目配置文件路径
        /// </summary>
        private static string GetConfigPath(string dllDir)
        {
            var files = Directory.GetFiles(dllDir);
            var configPath = files.Where(x => Path.GetExtension(x).ToLower().Equals(ProjectExt)).FirstOrDefault();
            if (configPath != null)
                return configPath;
            else
                return GetConfigPath(Path.GetDirectoryName(dllDir));
        }
    }
}
