using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace KSW.Helpers
{
    public static class ViewHelper
    {
        public static ViewModelBase GetViewModel(this IView view)
        {
            if (view == null) { throw new ArgumentNullException("view"); }

            object dataContext = view.DataContext;
            // When the DataContext is null then it might be that the ViewModel hasn't set it yet.
            // Enforce it by executing the event queue of the Dispatcher.
            if (dataContext == null && SynchronizationContext.Current is DispatcherSynchronizationContext)
            {
                dataContext = view.DataContext;
            }
            return dataContext as ViewModelBase;
        }

        public static T GetViewModel<T>(this IView view) where T : ViewModelBase
        {
            return GetViewModel(view) as T;
        }
    }
}
