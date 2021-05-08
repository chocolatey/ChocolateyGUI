// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="DialogService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using System.Windows;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Windows.Views;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.VisualStudio.Threading;

namespace ChocolateyGui.Common.Windows.Services
{
    public class DialogService : IDialogService
    {
        private readonly AsyncSemaphore _lock;

        public DialogService()
        {
            _lock = new AsyncSemaphore(1);
        }

        public ShellView ShellView { get; set; }

        /// <inheritdoc />
        public async Task<MessageDialogResult> ShowMessageAsync(string title, string message)
        {
            using (await _lock.EnterAsync())
            {
                if (ShellView != null)
                {
                    var dialogSettings = new MetroDialogSettings
                    {
                        AffirmativeButtonText = Resources.ChocolateyDialog_OK
                    };

                    return await ShellView.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, dialogSettings);
                }

                return MessageBox.Show(message, title) == MessageBoxResult.OK
                    ? MessageDialogResult.Affirmative
                    : MessageDialogResult.Negative;
            }
        }

        /// <inheritdoc />
        public async Task<MessageDialogResult> ShowConfirmationMessageAsync(string title, string message)
        {
            using (await _lock.EnterAsync())
            {
                if (ShellView != null)
                {
                    var dialogSettings = new MetroDialogSettings
                    {
                        AffirmativeButtonText = Resources.Dialog_Yes,
                        NegativeButtonText = Resources.Dialog_No
                    };

                    return await ShellView.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
                }

                return MessageBox.Show(message, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes
                    ? MessageDialogResult.Affirmative
                    : MessageDialogResult.Negative;
            }
        }
    }
}