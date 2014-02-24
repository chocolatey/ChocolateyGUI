using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Chocolatey.Gui.Base;
using Chocolatey.Gui.Controls;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.Views.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace Chocolatey.Gui.Services
{
    public class ProgressService : ObservableBase, IProgressService
    {
        public MainWindow MainWindow;

        public ProgressService()
        {
            _isLoading = false;
            _loadingItems = 0;

            _output = new ObservableRingBuffer<PowerShellOutputLine>(100);
            _output.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null && args.NewItems.Count > 0)
                {
                    foreach (PowerShellOutputLine newItem in args.NewItems)
                    {
                        Console.WriteLine(newItem.Text);
                    }
                }
            };

            Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                .Where(p => p.EventArgs.PropertyName == "IsLoading")
                .Select(_ => IsLoading)
                .Delay(TimeSpan.FromMilliseconds(500))
                .ObserveOnDispatcher()
                .Subscribe(async isLoading =>
                {
                    if (isLoading && IsLoading &&
                        MainWindow != null && _progressController == null)
                    {
                        _progressController = await MainWindow.ShowProgressAsync(ProgressTitle, ProgressMessage);
                        _progressController.SetIndeterminate();
                        return;
                    }

                    if (!isLoading && _progressController != null)
                    {
                        await _progressController.CloseAsync();
                        _progressController = null;
                    }
                });
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
        }


        private int _loadingItems;
        private ProgressDialogController _progressController;

        private string ProgressTitle { get; set; }
        private string ProgressMessage { get; set; }

        public void StartLoading(string title = "", string message = "")
        {
            var currentCount = Interlocked.Increment(ref _loadingItems);
            if (currentCount == 1)
            {
                ProgressTitle = title;
                ProgressMessage = message;

                _isLoading = true;
                NotifyPropertyChanged("IsLoading");
            }
        }

        public void StopLoading()
        {
            var currentCount = Interlocked.Decrement(ref _loadingItems);
            if (currentCount == 0)
            {
                _isLoading = false;
                _output.Clear();
                Report(0);
                NotifyPropertyChanged("IsLoading");
            }
        }

        private readonly ObservableRingBuffer<PowerShellOutputLine> _output;
        public ObservableRingBuffer<PowerShellOutputLine> Output
        {
            get { return _output; }
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

        public async Task<MessageDialogResult> ShowMessage(string title, string message)
        {
            if (MainWindow != null)
            {
                return await MainWindow.ShowMessageAsync(title, message);
            }
            return MessageBox.Show(message, title) == MessageBoxResult.OK ? MessageDialogResult.Affirmative : MessageDialogResult.Negative;
        }
    }
}
