using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.UI.WPF.Controls
{
    public interface IToastManager
    {
        /// <summary>
        /// Show a toast.
        /// </summary>
        /// <param name="toast">The toast to be displayed.</param>
        void Show(IToast toast);
    }
}
