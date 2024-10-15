/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：TemplateBLL.cs
// 功能描述：项目模板业务逻辑层
//
// 作者：zhangyingzhong
// 日期：2024/10/11 16:16
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Helpers
{
    /// <summary>
    /// 项目模板业务逻辑层
    /// </summary>
    public static class ProjectTemplateHelper
    {
        private static readonly string _commandExecute = "dotnet";

        public static async Task<bool> CreateSolutionByTemplateAsync(string targetDir, string templateName, bool isCover = true)
        {
            var result = false;
            if (!await IsTemplateInstalledAsync(templateName))
                return result;

            if (Directory.Exists(targetDir))
            {
                if (IsDirectoryEmpty(targetDir) || isCover)
                {
                    var commandExecute = "cmd.exe";

                    var nameSpace = Path.GetFileName(targetDir);
                    var commandParams = $"/C cd /d {targetDir} && {_commandExecute} new {templateName} -N {nameSpace} --force";
                    var output = await CommandLineHelper.SendCommandLine(commandExecute, commandParams);
                    result = true;
                }
            }

            return result;
        }

        public static async Task<bool> InstallTemplateAsync(string templatePath, bool isUpdate = false)
        {
            var result = false;

            if (Directory.Exists(templatePath))
            {
                var commandParams = $"new install {templatePath}";
                if (isUpdate)
                    commandParams = $"new install {templatePath} --force";
                var output = await CommandLineHelper.SendCommandLine(_commandExecute, commandParams);
                result = true;
            }
            return result;
        }

        public static async Task<bool> IsTemplateInstalledAsync(string templateName)
        {
            var commandParams = "new --list";
            var output = await CommandLineHelper.SendCommandLine(_commandExecute, commandParams);
            // 检查模板名称是否存在于输出中
            return output.Contains(templateName, StringComparison.OrdinalIgnoreCase);
        }

        public static async Task<bool> CopyProjectAsync(string sourceDir, string destinationDir)
        {
            var result = false;
            try
            {
                var commandExecute = "robocopy";
                var commandParams = $"{sourceDir} {destinationDir} /E /XF *.atecfg /XD bin obj";
                var output = await CommandLineHelper.SendCommandLine(commandExecute, commandParams);
                result = true;

            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }

        private static bool IsDirectoryEmpty(string path)
        {
            // 检查目录中是否有任何文件或子文件夹
            return Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0;
        }

    }
}
