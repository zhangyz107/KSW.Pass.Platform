/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：LanguageManager.cs
// 功能描述：语言管理类
//
// 作者：zhangyingzhong
// 日期：2025/01/09 16:57
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.Localization;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace KSW.ATE01.PatternCompiler.Start.Language
{
    /// <summary>
    /// 语言管理类
    /// </summary>
    public class LanguageManager : ILanguageManager
    {
        private readonly string _resource = "KSW.ATE01.PatternCompiler.Start.Properties.Resources";

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
