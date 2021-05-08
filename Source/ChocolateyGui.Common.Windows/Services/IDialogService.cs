// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IDialogService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using ChocolateyGui.Common.Windows.Views;
using MahApps.Metro.Controls.Dialogs;

namespace ChocolateyGui.Common.Windows.Services
{
    public interface IDialogService
    {
        ShellView ShellView { get; set; }

        /// <summary>
        /// Creates a Message dialog with an OK button inside of the ShellView.
        /// </summary>
        /// <param name="title">The title of the Dialog.</param>
        /// <param name="message">The message contained within the Dialog.</param>
        /// <returns>A task promising the result of which button was pressed.</returns>
        Task<MessageDialogResult> ShowMessageAsync(string title, string message);

        /// <summary>
        /// Creates a Message dialog with Yes/No buttons inside of the ShellView.
        /// </summary>
        /// <param name="title">The title of the Dialog.</param>
        /// <param name="message">The message contained within the Dialog.</param>
        /// <returns>A task promising the result of which button was pressed.</returns>
        Task<MessageDialogResult> ShowConfirmationMessageAsync(string title, string message);
    }
}