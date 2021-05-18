// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IDialogService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using ChocolateyGui.Common.Windows.Controls.Dialogs;
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

        /// <summary>
        /// Creates a Login dialog inside of the ShellView.
        /// </summary>
        /// <param name="title">The title of the Dialog.</param>
        /// <param name="message">The message contained within the Dialog.</param>
        /// <param name="settings">Optional settings that override the global dialog settings.</param>
        /// <returns>The text that was entered or null (Nothing in Visual Basic) if the user cancelled the operation.</returns>
        Task<LoginDialogData> ShowLoginAsync(string title, string message, LoginDialogSettings settings = null);

        /// <summary>
        /// Creates a Custom dialog inside of the ShellView.
        /// </summary>
        /// <param name="title">The title of the Dialog.</param>
        /// <param name="dialogContent">The content within the Dialog.</param>
        /// <param name="dialogContext">The context of the content within the Dialog.</param>
        /// <param name="settings">Optional settings that override the global dialog settings.</param>
        /// <typeparam name="TDialogContext">The type of the context.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>The text that was entered or null (Nothing in Visual Basic) if the user cancelled the operation.</returns>
        Task<TResult> ShowDialogAsync<TDialogContext, TResult>(string title, object dialogContent, TDialogContext dialogContext, MetroDialogSettings settings = null)
            where TDialogContext : IClosableDialog<TResult>;

        /// <summary>
        /// Creates a Custom child window inside of the ShellView.
        /// </summary>
        /// <param name="title">The title of the child window.</param>
        /// <param name="dialogContent">The content within the child window.</param>
        /// <param name="dialogContext">The context of the content within the child window.</param>
        /// <typeparam name="TDialogContext">The type of the context.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>The text that was entered or null (Nothing in Visual Basic) if the user cancelled the operation.</returns>
        Task<TResult> ShowChildWindowAsync<TDialogContext, TResult>(string title, object dialogContent, TDialogContext dialogContext)
            where TDialogContext : IClosableChildWindow<TResult>;
    }
}