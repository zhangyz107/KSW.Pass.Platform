/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：LanguageManager.cs
// 功能描述：多语言管理
//
// 作者：zhangyingzhong
// 日期：2024/10/09 13:41
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.Localization;
using Prism.Ioc;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Resources;

namespace KSW.ATE01.Start
{
    /// <summary>
    /// 多语言管理
    /// </summary>
    public class LanguageHelper
    {
        /// <summary>
        /// 多语言资源命名空间
        /// </summary>
        public string ResourceName
        {
            get => "KSW.ATE01.Start.Properties.Resources";
        }

        private readonly ILanguageManager _manager;

        private static readonly Lazy<LanguageHelper> _lazy = new Lazy<LanguageHelper>(() => new LanguageHelper());

        public static ILanguageManager Manager { get { return _lazy?.Value?._manager; } }

        public LanguageHelper()
        {
            _manager = LanguageManagerFactory.CreateManager(ResourceName, GetType().Assembly);
        }
    }
}
