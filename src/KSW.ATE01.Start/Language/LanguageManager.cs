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
    public class LanguageManager : ILanguageManager
    {
        private readonly string _resource = "KSW.ATE01.Start.Properties.Resources";

        private readonly ResourceManager _resourceManager;

        private static readonly Lazy<LanguageManager> _lazy = new Lazy<LanguageManager>(() => new LanguageManager());

        public event PropertyChangedEventHandler PropertyChanged;

        public static LanguageManager Instance { get { return _lazy.Value; } }

        public LanguageManager()
        {
            _resourceManager = new ResourceManager(_resource, GetType().Assembly);
            CultureManager.CurrentCultureChanged += CultureManager_CurrentCultureChanged;
        }

        public string this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }
                return _resourceManager.GetString(name);
            }
        }

        /// <summary>
        /// 切换使用语言
        /// </summary>
        /// <param name="cultureInfo"></param>
        public void ChangeLanguage(CultureInfo cultureInfo)
        {
            CultureManager.CurrentCulture = cultureInfo;
        }


        private void CultureManager_CurrentCultureChanged(object? sender, CultureInfo e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("item[]"));
        }

    }
}
