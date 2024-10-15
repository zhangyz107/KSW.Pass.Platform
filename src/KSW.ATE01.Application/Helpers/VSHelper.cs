
/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：VSHelper.cs
// 功能描述：VS帮助类
//
// 作者：zhangyingzhong
// 日期：2024/10/14 17:18
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using EnvDTE;
using EnvDTE80;
using KSW.Exceptions;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KSW.ATE01.Application.Helpers
{
    /// <summary>
    /// VS帮助类（仅支持Visual Studio 2022）
    /// </summary>
    public class VSHelper
    {
        private static readonly string _vs2022 = "VisualStudio.DTE.17.0";

        /// <summary>
        /// 重命名解决方案和项目
        /// </summary>
        /// <param name="slnPath">解决方案路径</param>
        /// <param name="oldProjectName">原项目名称</param>
        /// <param name="newProjectName">重命名项目名称</param>
        /// <returns></returns>
        public static async Task<bool> RenameProjctAsync(string slnPath, string oldProjectName, string newProjectName)
        {
            var result = false;
            await Task.Factory.StartNew(() =>
            {
                // 创建 DTE 实例
                var dte = (DTE2)Activator.CreateInstance(Type.GetTypeFromProgID(_vs2022));
                if (dte == null)
                    throw new Warning("未找到Visual Stdio 2022");

                dte.MainWindow.Visible = false; // 隐藏主窗口
                try
                {
                    dte.Solution.Open(slnPath);

                    var project = dte.Solution.Projects.Cast<Project>().FirstOrDefault(x => x.Name == oldProjectName);

                    if (project != null)
                    {
                        project.Name = newProjectName;
                        project.Save();
                    }

                    dte.Solution.Close(false);
                    result = true;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    // 退出 Visual Studio
                    dte.Quit();
                    Marshal.ReleaseComObject(dte); // 释放 DTE 对象
                }
            });

            return result;
        }


        /// <summary>
        /// 重命名解决方案和项目
        /// </summary>
        /// <param name="slnPath">解决方案路径</param>
        /// <param name="newSlnName">解决方案重命名</param>
        /// <param name="oldProjectName">原项目名称</param>
        /// <param name="newProjectName">重命名项目名称</param>
        /// <returns></returns>
        public static async Task<bool> RenameSolutionAndProjctAsync(string slnPath, string newSlnName, string oldProjectName, string newProjectName)
        {
            var result = false;
            await Task.Factory.StartNew(() =>
             {
                 // 创建 DTE 实例
                 var dte = (DTE2)Activator.CreateInstance(Type.GetTypeFromProgID(_vs2022));
                 if (dte == null)
                     throw new Warning("未找到Visual Stdio 2022");

                 dte.MainWindow.Visible = false; // 隐藏主窗口
                 try
                 {
                     dte.Solution.Open(slnPath);

                     var project = dte.Solution.Projects.Cast<Project>().FirstOrDefault(x => x.Name == oldProjectName);

                     if (project != null)
                     {
                         project.Name = newProjectName;
                         project.Save();
                     }

                     dte.Solution.SaveAs(newSlnName);
                     dte.Solution.Close(false);
                     result = true;
                 }
                 catch (Exception)
                 {
                     throw;
                 }
                 finally
                 {
                     // 退出 Visual Studio
                     dte.Quit();
                     Marshal.ReleaseComObject(dte); // 释放 DTE 对象

                     // 删除原解决方案
                     if (result && File.Exists(slnPath))
                         File.Delete(slnPath);
                 }
             });

            return result;
        }
    }
}
