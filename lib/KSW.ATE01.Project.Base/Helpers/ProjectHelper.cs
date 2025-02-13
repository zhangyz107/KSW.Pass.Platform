using KSW.ATE01.Project.Base.Models.Projects;
using System.IO;
using System.Reflection;

namespace KSW.ATE01.Project.Base.Helpers
{
    /// <summary>
    /// 项目帮助类
    /// </summary>
    public class ProjectHelper
    {
        private static readonly string _projectExt = ".atecfg";

        /// <summary>
        /// 加载项目信息
        /// </summary>
        /// <returns></returns>
        public static ProjectInfo LoadProjectInfo()
        {
            try
            {
                var dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var configPath = GetConfigPath(dllDir);
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
            var configPath = files.Where(x => Path.GetExtension(x).ToLower().Equals(_projectExt)).FirstOrDefault();
            if (configPath != null)
                return configPath;
            else
                return GetConfigPath(Path.GetDirectoryName(dllDir));
        }
    }
}
