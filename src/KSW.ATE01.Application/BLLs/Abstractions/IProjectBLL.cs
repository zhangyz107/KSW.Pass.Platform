﻿using KSW.Application;
using KSW.ATE01.Application.Models.Projects;

namespace KSW.ATE01.Application.BLLs.Abstractions
{
    /// <summary>
    /// 项目业务逻辑层接口
    /// </summary>
    public interface IProjectBLL : IService
    {
        /// <summary>
        /// 创建项目
        /// </summary>
        Task CreateProject(ProjectInfoModel projectInfo);

        /// <summary>
        /// 获取当前项目
        /// </summary>
        ProjectInfoModel GetCurrentProjectInfo();
    }
}
