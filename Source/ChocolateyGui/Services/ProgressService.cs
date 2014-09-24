// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ProgressService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using ChocolateyGui.Base;
    using ChocolateyGui.Controls;
    using ChocolateyGui.Controls.Dialogs;
    using ChocolateyGui.Models;
    using ChocolateyGui.Utilities;
    using ChocolateyGui.Views.Windows;
    using MahApps.Metro.Controls.Dialogs;

    public class ProgressService : ObservableBase, IProgressService
    {
        private readonly AsyncLock _lock;
        private readonly ObservableRingBuffer<PowerShellOutputLine> _output;
        private CancellationTokenSource _cst = null;
        private bool _isLoading;
        private int _loadingItems;
        private double _progress;
        private ChocolateyDialogController _progressController;
        private MainWindow mainWindow;

        public ProgressService()
        {
            this._isLoading = false;
            this._loadingItems = 0;
            this._output = new ObservableRingBuffer<PowerShellOutputLine>(100);
            this._lock = new AsyncLock();
        }

        public bool IsLoading
        {
            get
            {
                return this._isLoading;
            }
        }

        public MainWindow MainWindow
        {
            get
            {
                return this.mainWindow;
            }

            set
            {
                this.mainWindow = value;
            }
        }

        public ObservableRingBuffer<PowerShellOutputLine> Output
        {
            get
            {
                return this._output;
            }
        }

        public double Progress
        {
            get
            {
                return this._progress;
            }
        }

        public CancellationToken GetCancellationToken()
        {
            if (!this.IsLoading)
            {
                throw new InvalidOperationException("There's no current operation in process.");
            }

            return this._cst.Token;
        }

        public void Report(double value)
        {
            this._progress = value;

            if (this._progressController != null)
            {
                if (value < 0)
                {
                    this._progressController.SetIndeterminate();
                }
                else
                {
                    this._progressController.SetProgress(Math.Min(this._progress / 100.0f, 100));
                }
            }

            this.NotifyPropertyChanged("Progress");
        }

        public async Task<MessageDialogResult> ShowMessageAsync(string title, string message)
        {
            if (this.MainWindow != null)
            {
                return await this.MainWindow.ShowMessageAsync(title, message);
            }

            return MessageBox.Show(message, title) == MessageBoxResult.OK ? MessageDialogResult.Affirmative : MessageDialogResult.Negative;
        }

        public async Task StartLoading(string title = null, bool isCancelable = false)
        {
            using (await this._lock.LockAsync())
            {
                var currentCount = Interlocked.Increment(ref this._loadingItems);
                if (currentCount == 1)
                {
                    var chocoDialg = new ChocolateyDialog(this.MainWindow);

                    this._progressController = await this.MainWindow.ShowChocolateyDialogAsync(title, isCancelable);
                    this._progressController.SetIndeterminate();
                    if (isCancelable)
                    {
                        this._cst = new CancellationTokenSource();
                        this._progressController.OnCanceled += dialog =>
                        {
                            if (this._cst != null)
                            {
                                _cst.Cancel();
                            }
                        };
                    }

                    this._output.Clear();

                    this._isLoading = true;
                    this.NotifyPropertyChanged("IsLoading");
                }
            }
        }

        public async Task StopLoading()
        {
            using (await this._lock.LockAsync())
            {
                var currentCount = Interlocked.Decrement(ref this._loadingItems);
                if (currentCount == 0)
                {
                    await this._progressController.CloseAsync();
                    this._progressController = null;
                    this.Report(0);

                    this._isLoading = false;
                    this.NotifyPropertyChanged("IsLoading");
                }
            }
        }

        public void WriteMessage(string message, PowerShellLineType type = PowerShellLineType.Output, bool newLine = true)
        {
            this._output.Add(new PowerShellOutputLine(message, type, newLine));
        }
    }
}