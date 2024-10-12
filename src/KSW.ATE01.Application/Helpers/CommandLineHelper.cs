/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：CommandLineHelper.cs
// 功能描述：命令行帮助类
//
// 作者：zhangyingzhong
// 日期：2024/10/11 16:10
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Helpers
{
    /// <summary>
    /// 命令行帮助类
    /// </summary>
    public static class CommandLineHelper
    {
        /// <summary>
        /// 发送命令行命令
        /// </summary>
        /// <param name="commandExecute"></param>
        /// <param name="commandParams"></param>
        /// <returns></returns>
        public static async Task<string> SendCommandLine(string commandExecute, string commandParams)
        {
            if (commandExecute == null)
                return string.Empty;

            var processStartInfo = new ProcessStartInfo()
            {
                FileName = commandExecute,
                Arguments = commandParams,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            //启动进程
            using (Process process = Process.Start(processStartInfo))
            {
                if (process == null)
                    return string.Empty;

                // 读取输出内容
                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();
                return output;
            }
        }

        /// <summary>
        /// 发送命令行命令
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static async Task<string> SendCommandLine(ProcessStartInfo info)
        {
            if (info == null)
                return string.Empty;

            //启动进程
            using (Process process = Process.Start(info))
            {
                if (process == null)
                    return string.Empty;

                // 读取输出内容
                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();
                return output;
            }
        }
    }
}
