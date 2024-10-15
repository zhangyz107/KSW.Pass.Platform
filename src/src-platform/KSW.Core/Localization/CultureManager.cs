using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.Localization
{
    public class CultureManager
    {
        private static CultureInfo _currentCulture;

        public static event EventHandler<CultureInfo> CurrentCultureChanged;

        public static CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                if (_currentCulture != value)
                {
                    _currentCulture = value;
                    CultureInfo.CurrentCulture = value;
                    CultureInfo.CurrentUICulture = value;
                    OnCurrentCultureChanged(value);
                }
            }
        }

        protected static void OnCurrentCultureChanged(CultureInfo newCulture)
        {
            CurrentCultureChanged?.Invoke(null, newCulture);
        }
    }
}
