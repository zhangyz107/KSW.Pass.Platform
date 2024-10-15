using KSW.Data;

namespace KSW.Helpers
{
    public static class ProcessBarHelper
    {
        public static string GetProcessBarDialogName => "ProcessBarDialog";

        public static ProcessBarParameters CreateProcessBarParameters(Func<Action<double, string>, Task> doWork)
        {
            return CreateProcessBarParameters(doWork, true, 0, null);
        }

        public static ProcessBarParameters CreateProcessBarParameters(Func<Action<double, string>, Task> doWork, bool isIndeterminate)
        {
            return CreateProcessBarParameters(doWork, isIndeterminate, 0, null);
        }

        public static ProcessBarParameters CreateProcessBarParameters(Func<Action<double, string>, Task> doWork, bool isIndeterminate, double processRate)
        {
            return CreateProcessBarParameters(doWork, isIndeterminate, processRate, null);
        }

        public static ProcessBarParameters CreateProcessBarParameters(Func<Action<double, string>, Task> doWork, bool isIndeterminate, double processRate, string processContent)
        {
            var parameters = new ProcessBarParameters()
            {
                IsIndeterminate = isIndeterminate,
                ProcessContent = processContent,
                ProcessRate = processRate,

            };
            parameters.DoWork = () => doWork.Invoke(parameters.UpdateProgress);
            return parameters;
        }


        public static async Task<IDialogResult> ShowProcessBarDialogAsync(this IDialogService dialogService, ProcessBarParameters processBarParameters)
        {
            var tempParams = new DialogParameters
            {
                { "params", processBarParameters }
            };
            return await dialogService.ShowDialogAsync(GetProcessBarDialogName, tempParams);
        }

        public static void ShowProcessBar(this IDialogService dialogService, ProcessBarParameters processBarParameters, Action<IDialogResult> callback = null)
        {
            var tempParams = new DialogParameters
            {
                { "params", processBarParameters }
            };
            dialogService.Show(GetProcessBarDialogName, tempParams, callback);
        }
    }
}
