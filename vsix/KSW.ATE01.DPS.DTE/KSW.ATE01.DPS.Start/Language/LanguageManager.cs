using KSW.Localization;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace KSW.ATE01.DPS.Start.Language
{
    /// <summary>
    /// 多语言管理
    /// </summary>
    public class LanguageManager : ILanguageManager
    {
        private readonly string _resource = "KSW.ATE01.DPS.Start.Properties.Resources";

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
