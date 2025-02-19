using KSW.ATE01.Instrument.IO.BLLs.Implements.Patterns;
using KSW.ATE01.Project.Base.Models;
using System.IO;

namespace CustomerProgram
{
    public class TemplateDigital
    {
        /// <summary>
        /// 模式文件路径是相对的
        /// </summary>
        /// <param name="patternFiles"></param>
        /// <param name="isFullPath"></param>
        public static void LoadPatterns(string[] patternFiles, bool isFullPath = false)
        {
            try
            {
                if (patternFiles == null && !patternFiles.Any())
                    return;

                var commonData = CommonData.Instance;
                if (commonData?.ProjectInfo != null && !string.IsNullOrEmpty(commonData?.ProjectInfo?.ReleasePath) && Directory.Exists(commonData?.ProjectInfo?.ReleasePath))
                {
                    var patternFilePaths = new List<string>();
                    foreach (string patternFile in patternFiles)
                    {
                        string loadFile = "";
                        if (isFullPath)
                            loadFile = patternFile;
                        else
                        {
                            loadFile = Path.Combine(commonData?.ProjectInfo?.ReleasePath, "Patterns", patternFile);

                            if (File.Exists(loadFile))
                            {
                                patternFilePaths.Add(loadFile);
                            }
                        }
                    }
                    var patternArray = patternFilePaths.ToArray();
                    Pattern.SetPatternFile(patternArray);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
