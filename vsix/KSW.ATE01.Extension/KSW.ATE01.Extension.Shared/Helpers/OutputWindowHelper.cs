using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using KSW.ATE01.Extension.Shared.Properties;
using System;
using KSW.ATE01.Extension.VS2022;

namespace KSW.ATE01.Extension.Shared.Helpers
{
    /// <summary>
    /// A helper class for writing messages to a Pass output window pane.
    /// </summary>
    internal static class OutputWindowHelper
    {
        #region Fields

        private static IVsOutputWindowPane _passOutputWindowPane;

        #endregion Fields

        #region Properties

        private static IVsOutputWindowPane PassOutputWindowPane =>
            _passOutputWindowPane ?? (_passOutputWindowPane = GetATE01OutputWindowPane());

        #endregion Properties

        #region Methods

        /// <summary>
        /// Writes the specified diagnostic line to the Pass output pane, but only if diagnostics are enabled.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">An optional exception that was handled.</param>
        internal static void DiagnosticWriteLine(string message, Exception ex = null)
        {
            if (!Settings.Default.General_DiagnosticsMode) return;

            if (ex != null)
            {
                message += $": {ex}";
            }

            WriteLine(Resources.Diagnostic, message);
        }

        /// <summary>
        /// Writes the specified exception line to the Pass output pane.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The exception that was handled.</param>
        internal static void ExceptionWriteLine(string message, Exception ex)
        {
            var exceptionMessage = $"{message}: {ex}";

            WriteLine(Resources.HandledException, exceptionMessage);
        }

        /// <summary>
        /// Writes the specified warning line to the Pass output pane.
        /// </summary>
        /// <param name="message">The message.</param>
        internal static void WarningWriteLine(string message)
        {
            WriteLine(Resources.Warning, message);
        }

        /// <summary>
        /// Attempts to create and retrieve the Pass output window pane.
        /// </summary>
        /// <returns>The Pass output window pane, otherwise null.</returns>
        private static IVsOutputWindowPane GetATE01OutputWindowPane()
        {
            if (!(Package.GetGlobalService(typeof(SVsOutputWindow)) is IVsOutputWindow outputWindow))
            {
                return null;
            }

            Guid outputPaneGuid = new Guid(ATE01Guids.GuidATE01OutputPanel.ToByteArray());

            outputWindow.CreatePane(ref outputPaneGuid, "ATE01", 1, 1);
            outputWindow.GetPane(ref outputPaneGuid, out IVsOutputWindowPane windowPane);

            return windowPane;
        }

        /// <summary>
        /// Writes the specified line to the Pass output pane.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="message">The message.</param>
        private static void WriteLine(string category, string message)
        {
            var outputWindowPane = PassOutputWindowPane;
            if (outputWindowPane != null)
            {
                string outputMessage = $"[ATE01 {category} {DateTime.Now.ToString("hh:mm:ss tt")}] {message}{Environment.NewLine}";

                outputWindowPane.OutputString(outputMessage);
            }
        }

        #endregion Methods
    }
}