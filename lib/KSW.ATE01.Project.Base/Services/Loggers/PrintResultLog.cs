using KSW.ATE01.Project.Base.Events;
using KSW.ATE01.Project.Base.Models.Errors;
using KSW.ATE01.Project.Base.Models.Loggers;
using KSW.ATE01.Project.Base.Services.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Services.Loggers
{
    public class PrintResultLog
    {
        #region Fields
        private static readonly Lazy<PrintResultLog> _lazy = new Lazy<PrintResultLog>(() => new PrintResultLog());
        private static bool _printRealTimeTxt = false;
        private static IEventAggregator _eventAggregator;
        #endregion

        #region Properties
        public static bool PrintRealTimeTxt
        {
            get => _printRealTimeTxt;
            set => _printRealTimeTxt = value;
        }

        public static PrintResultLog Instance
        {
            get { return _lazy.Value; }
        }
        #endregion

        public PrintResultLog()
        {
            var container = ContainerLocator.Container;
            _eventAggregator = container?.Resolve<IEventAggregator>() ?? null;
        }

        public static void Message(string message)
        {
            _eventAggregator.GetEvent<PrintToRealTimeTxtEvent>().Publish(new RealTimeMessage()
            {
                Message = message
            });
        }

        public static void Pattern(string pattern)
        {

        }

        public static void PrintErrorMessage(string errorMessage)
        {
            Message(errorMessage);
        }

        public static void PrintRealTimeWithTestItem()
        {

        }

        public static void PrintRealTimeTextWithSummary()
        {

        }
    }
}
