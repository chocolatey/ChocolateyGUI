using System;
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
        public  MainWindow MainWindow;
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
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
        }

        private int _loadingItems;
        public void StartLoading(string title = "", string message = "")
        {
            lock (this)
            {
                Interlocked.Increment(ref _loadingItems);
                if (_isLoading == false)
                {
                    _isLoading = true;
                    NotifyPropertyChanged("IsLoading");
                }
            }
        }

        public void StopLoading()
        {
            lock (this)
            {
                if (_isLoading && Interlocked.Decrement(ref _loadingItems) < 1)
                {
                    _isLoading = false;
                    _output.Clear();
                    Report(0);
                    NotifyPropertyChanged("IsLoading");
                }
            }
        }

        private readonly ObservableRingBuffer<PowerShellOutputLine> _output;
        public ObservableRingBuffer<PowerShellOutputLine> Output
        {
            get { return _output; }
        }

        private int _progress;
        public int Progress
        {
            get { return _progress; }
        }

        public void Report(int value)
        {
            Interlocked.Exchange(ref _progress, value);
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
