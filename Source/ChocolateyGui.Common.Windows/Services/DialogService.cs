// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="DialogService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Windows.Controls.Dialogs;
using ChocolateyGui.Common.Windows.Views;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using Microsoft.VisualStudio.Threading;

namespace ChocolateyGui.Common.Windows.Services
{
    public class DialogService : IDialogService
    {
        private readonly AsyncSemaphore _lock;
        private EventHandler<ThemeChangedEventArgs> _themeChangedHandler = null;
        private RoutedEventHandler _childWindowLoadedHandler = null;

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

        /// <inheritdoc />
        public async Task<LoginDialogData> ShowLoginAsync(string title, string message, LoginDialogSettings settings = null)
        {
            using (await _lock.EnterAsync())
            {
                if (ShellView != null)
                {
                    return await ShellView.ShowLoginAsync(
                        Resources.SettingsViewModel_SetSourceUsernameAndPasswordTitle,
                        Resources.SettingsViewModel_SetSourceUsernameAndPasswordMessage,
                        settings);
                }

                return null;
            }
        }

        /// <inheritdoc />
        public async Task<TResult> ShowDialogAsync<TDialogContext, TResult>(
            string title,
            object dialogContent,
            TDialogContext dialogContext,
            MetroDialogSettings settings = null)
            where TDialogContext : IClosableDialog<TResult>
        {
            using (await _lock.EnterAsync())
            {
                if (ShellView != null)
                {
                    var customDialog = new CustomDialog
                    {
                        Title = title,
                        Content = dialogContent,
                        DialogContentMargin = new GridLength(1, GridUnitType.Star),
                        DialogContentWidth = GridLength.Auto
                    };

                    await ShellView.ShowMetroDialogAsync(customDialog, settings);

                    var result = await dialogContext.WaitForClosingAsync();

                    await ShellView.HideMetroDialogAsync(customDialog, settings);

                    return result;
                }

                return default;
            }
        }

        /// <inheritdoc />
        public async Task<TResult> ShowChildWindowAsync<TDialogContext, TResult>(
            string title,
            object dialogContent,
            TDialogContext dialogContext)
            where TDialogContext : IClosableChildWindow<TResult>
        {
            using (await _lock.EnterAsync())
            {
                if (ShellView != null)
                {
                    var overlayBrush = new SolidColorBrush(((SolidColorBrush) ShellView.OverlayBrush).Color)
                    {
                        Opacity = ShellView.OverlayOpacity
                    };
                    overlayBrush.Freeze();

                    var childWindow = new ChildWindow
                    {
                        Title = title,
                        Content = dialogContent,
                        DataContext = dialogContext,
                        IsModal = true,
                        AllowMove = true,
                        ShowCloseButton = true,
                        BorderThickness = new Thickness(1),
                        OverlayBrush = overlayBrush
                    };

                    childWindow.SetResourceReference(Control.BorderBrushProperty, "MahApps.Brushes.Highlight");

                    _childWindowLoadedHandler = (sender, e) =>
                    {
                        var cw = (ChildWindow)sender;

                        if (cw.DataContext is IClosableChildWindow<TResult> vm)
                        {
                            vm.Close += r => { cw.Close(r); };
                        }
                        else
                        {
                            cw.Close();
                        }
                    };

                    _themeChangedHandler = (s, e) =>
                    {
                        if (ShellView.OverlayBrush is SolidColorBrush brush)
                        {
                            overlayBrush = new SolidColorBrush(brush.Color)
                            {
                                Opacity = ShellView.OverlayOpacity
                            };
                            overlayBrush.Freeze();

                            childWindow.OverlayBrush = overlayBrush;
                        }
                    };

                    childWindow.Loaded += _childWindowLoadedHandler;
                    ThemeManager.Current.ThemeChanged += _themeChangedHandler;

                    var result = await ShellView.ShowChildWindowAsync<TResult>(childWindow);

                    childWindow.Loaded -= _childWindowLoadedHandler;
                    ThemeManager.Current.ThemeChanged -= _themeChangedHandler;

                    return result;
                }

                return default;
            }
        }
    }
}