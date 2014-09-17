using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ChocolateyGui.Base;
using ChocolateyGui.Controls;
using ChocolateyGui.Models;
using ChocolateyGui.Views.Windows;
using MahApps.Metro.Controls.Dialogs;
using ChocolateyGui.Controls.Dialogs;
using ChocolateyGui.Utilities;

namespace ChocolateyGui.Services
{
    public class ProgressService : ObservableBase, IProgressService
    {
        public MainWindow MainWindow;

        private readonly AsyncLock _lock;

        public ProgressService()
        {
            _isLoading = false;
            _loadingItems = 0;
            _output = new ObservableRingBuffer<PowerShellOutputLine>(100);
            _lock = new AsyncLock();
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
        }

        private int _loadingItems;
        private ChocolateyDialogController _progressController;
        private CancellationTokenSource _cst = null;

        public async Task StartLoading(string title = null, bool isCancelable = false)
        {
            using (await _lock.LockAsync())
            {
                var currentCount = Interlocked.Increment(ref _loadingItems);
                if (currentCount == 1)
                {
                    var chocoDialg = new ChocolateyDialog(MainWindow);

                    _progressController = await MainWindow.ShowChocolateyDialogAsync(title, isCancelable);
                    _progressController.SetIndeterminate();
                    if (isCancelable)
                    {
                        _cst = new CancellationTokenSource();
                        _progressController.OnCancelled += dialog =>
                        {
                            if (_cst != null)
                                _cst.Cancel();
                        };
                    }

                    _output.Clear();

                    _isLoading = true;
                    NotifyPropertyChanged("IsLoading");
                }
            }
        }

        public async Task StopLoading()
        {
            using (await _lock.LockAsync())
            {
                var currentCount = Interlocked.Decrement(ref _loadingItems);
                if (currentCount == 0)
                {
                    await _progressController.CloseAsync();
                    _progressController = null;
                    Report(0);

                    _isLoading = false;
                    NotifyPropertyChanged("IsLoading");
                }
            }
        }

        public CancellationToken GetCancellationToken()
        {
            if (!IsLoading)
                throw new InvalidOperationException("There's no current operation in process.");
            return _cst.Token;
        }

        private readonly ObservableRingBuffer<PowerShellOutputLine> _output;
        public ObservableRingBuffer<PowerShellOutputLine> Output
        {
            get { return _output; }
        }


        public void WriteMessage(string message, PowerShellLineType type = PowerShellLineType.Output, bool newLine = true)
        {
            _output.Add(new PowerShellOutputLine(message, type, newLine));
        }

        private double _progress;
        public double Progress
        {
            get { return _progress; }
        }

        public void Report(double value)
        {
            _progress = value;
            if (_progressController != null)
            {
                if(value < 0)
                    _progressController.SetIndeterminate();
                else
                    _progressController.SetProgress(Math.Min((_progress)/100.0f, 100));
            }
            NotifyPropertyChanged("Progress");
        }

        public async Task<MessageDialogResult> ShowMessageAsync(string title, string message)
        {
            if (MainWindow != null)
            {
                return await MainWindow.ShowMessageAsync(title, message);
            }
            return MessageBox.Show(message, title) == MessageBoxResult.OK ? MessageDialogResult.Affirmative : MessageDialogResult.Negative;
        }
    }
}
