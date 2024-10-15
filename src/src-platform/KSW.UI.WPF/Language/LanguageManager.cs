using KSW.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace KSW.UI.WPF.Language
{
    /// <summary>
    /// 多语言管理
    /// </summary>
    public class LanguageManager
    {
        private readonly string _resource = "KSW.UI.WPF.Properties.Resources";
        private readonly ResourceManager _resourceManager;
        private static readonly Lazy<LanguageManager> _lazy = new Lazy<LanguageManager>(() => new LanguageManager());

        public event PropertyChangedEventHandler PropertyChanged;
        public static LanguageManager Instance { get { return _lazy.Value; } }

        public LanguageManager()
        {
            _resourceManager = new ResourceManager(_resource, GetType().Assembly);
            CultureManager.CurrentCultureChanged += CultureManager_CurrentCultureChanged;
        }

        private void CultureManager_CurrentCultureChanged(object? sender, CultureInfo e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("item[]"));
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
    }
}
