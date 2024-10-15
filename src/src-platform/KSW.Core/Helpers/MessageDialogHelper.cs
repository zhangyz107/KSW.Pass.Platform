using KSW.Data;
using Prism.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KSW.Helpers
{
    public static class MessageDialogHelper
    {
        public static string GetMessageBoxDialogName => "MessageDialog";

        public static async Task<IDialogResult> ShowMessageDialog(this IDialogService dialogService, string messageText)
        {
            try
            {
               var dialogParameters = GetMessageDialogParameters(messageText);

                return await ShowMessageDialog(dialogService, dialogParameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public static async Task<IDialogResult> ShowMessageDialog(this IDialogService dialogService, string messageText, MessageBoxButton button)
        {
            var dialogParameters = GetMessageDialogParameters(messageText, button);

            return await ShowMessageDialog(dialogService, dialogParameters);
        }

        public static async Task<IDialogResult> ShowMessageDialog(this IDialogService dialogService, string messageText, MessageBoxButton button, MessageBoxImage icon)
        {
            var dialogParameters = GetMessageDialogParameters(messageText, button, icon);

            return await ShowMessageDialog(dialogService, dialogParameters);
        }

        private static MessageDialogParameters GetMessageDialogParameters(string messageText = null, MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None)
        {
            return new MessageDialogParameters()
            {
                MessageText = messageText,
                MessageButton = button,
                MessageIcon = icon
            };
        }

        private static async Task<IDialogResult> ShowMessageDialog(this IDialogService dialogService, MessageDialogParameters dialogParameters)
        {
            var tempParams = new DialogParameters
            {
                { "params", dialogParameters }
            };
            return await dialogService.ShowDialogAsync(GetMessageBoxDialogName, tempParams);
        }
    }
}
