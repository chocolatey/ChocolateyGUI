// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ProgressService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using ChocolateyGui.Common.Base;
using ChocolateyGui.Common.Controls;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Windows.Controls.Dialogs;
using ChocolateyGui.Common.Windows.Utilities.Extensions;
using ChocolateyGui.Common.Windows.Views;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.VisualStudio.Threading;
using Serilog;
using Serilog.Events;

namespace ChocolateyGui.Common.Windows.Services
{
    public class ProgressService : ObservableBase, IProgressService
    {
        private readonly AsyncSemaphore _lock;
        private CancellationTokenSource _cst;
        private int _loadingItems;
        private ChocolateyDialogController _progressController;

        public ProgressService()
        {
            IsLoading = false;
            _loadingItems = 0;
            Output = new ObservableRingBufferCollection<PowerShellOutputLine>(100);
            _lock = new AsyncSemaphore(1);
        }

        public ShellView ShellView { get; set; }

        public double Progress { get; private set; }

        public bool IsLoading { get; private set; }

        public ObservableRingBufferCollection<PowerShellOutputLine> Output { get; }

        public CancellationToken GetCancellationToken()
        {
            if (!IsLoading)
            {
                throw new InvalidOperationException("There's no current operation in process.");
            }

            return _cst.Token;
        }

        public void Report(double value)
        {
            Progress = value;

            if (_progressController != null)
            {
                if (value < 0)
                {
                    Execute.OnUIThread(() => _progressController.SetIndeterminate());
                }
                else
                {
                    Execute.OnUIThread(() => _progressController.SetProgress(Math.Min(Progress / 100.0f, 100)));
                }
            }

            NotifyPropertyChanged("Progress");
        }

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

                    return await RunOnUIAsync(() => ShellView.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, dialogSettings));
                }

                return MessageBox.Show(message, title) == MessageBoxResult.OK
                           ? MessageDialogResult.Affirmative
                           : MessageDialogResult.Negative;
            }
        }

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

                    return await RunOnUIAsync(() => ShellView.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative, dialogSettings));
                }

                return MessageBox.Show(message, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes
                           ? MessageDialogResult.Affirmative
                           : MessageDialogResult.Negative;
            }
        }

        public async Task StartLoading(string title = null, bool isCancelable = false)
        {
            using (await _lock.EnterAsync())
            {
                var currentCount = Interlocked.Increment(ref _loadingItems);
                if (currentCount == 1)
                {
                    await RunOnUIAsync(async () =>
                    {
                        _progressController = await ShellView.ShowChocolateyDialogAsync(title, isCancelable);
                        _progressController.SetIndeterminate();
                        if (isCancelable)
                        {
                            _cst = new CancellationTokenSource();
                            _progressController.OnCanceled += dialog =>
                            {
                                if (_cst != null)
                                {
                                    _cst.Cancel();
                                }
                            };
                        }

                        Output.Clear();

                        IsLoading = true;
                        NotifyPropertyChanged("IsLoading");
                    });
                }
            }
        }

        public async Task StopLoading()
        {
            using (await _lock.EnterAsync())
            {
                var currentCount = Interlocked.Decrement(ref _loadingItems);
                if (currentCount == 0)
                {
                    await RunOnUIAsync(() => _progressController.CloseAsync());
                    _progressController = null;
                    Report(0);

                    IsLoading = false;
                    NotifyPropertyChanged("IsLoading");
                }
            }
        }

        public void WriteMessage(
            string message,
            PowerShellLineType type = PowerShellLineType.Output,
            bool newLine = true)
        {
            // Don't show debug events when not running in debug.
            if (type == PowerShellLineType.Debug && !Log.IsEnabled(LogEventLevel.Debug))
            {
                return;
            }

            Execute.BeginOnUIThread(() => Output.Add(new PowerShellOutputLine(message, type, newLine)));
        }

        private static Task RunOnUIAsync(Func<Task> action)
        {
            return action.RunOnUIThreadAsync();
        }

        private static Task<T> RunOnUIAsync<T>(Func<Task<T>> action)
        {
            return action.RunOnUIThreadAsync();
        }
    }
}