using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace KSW.UI.WPF.Controls
{
    public class ToastDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ToastTemplate { get; set; }
        public DataTemplate StringTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IToast)
                return ToastTemplate;
            else
                return StringTemplate ?? base.SelectTemplate(item, container); 
        }
    }
}
