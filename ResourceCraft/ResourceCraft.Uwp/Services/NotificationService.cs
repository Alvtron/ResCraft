using ResourceCraft.Uwp.Dialogs;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ResourceCraft.Uwp.Services
{
    /// <summary>
    /// 
    /// </summary>
    public static class NotificationService
    {
        public static async Task<ContentDialogResult> DisplayGeneralMessage(string title, string message, string primaryButtonText = "", string secondaryButtonText = "", string closeButtonText = "")
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = primaryButtonText,
                SecondaryButtonText = secondaryButtonText,
                CloseButtonText = closeButtonText
            };

            return await dialog.ShowAsync();
        }

        public static async Task<ContentDialogResult> DisplayConfirmationDialog(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "Allow",
                CloseButtonText = "Deny"
            };

            return await dialog.ShowAsync();
        }

        public static async Task<ContentDialogResult> DisplayLoadingDialog(int timeInSeconds)
        {
            var dialog = new LoadingDialog(timeInSeconds);
            return await dialog.ShowAsync();
        }

        public static async Task<LoadingDialog> CreateAndDisplayLoadingDialog()
        {
            var dialog = new LoadingDialog();
            await dialog.ShowAsync();
            return dialog;
        }

        /// <summary>
        /// Displays the error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static async Task<ContentDialogResult> DisplayErrorMessage(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Woah, there!",
                Content = message,
                PrimaryButtonText = "OK"
            };

            return await dialog.ShowAsync();
        }

        /// <summary>
        /// Displays the thank you message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static async Task<ContentDialogResult> DisplayThankYouMessage(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Thank you!",
                Content = message,
                PrimaryButtonText = "OK"
            };

            return await dialog.ShowAsync();
        }
    }
}
