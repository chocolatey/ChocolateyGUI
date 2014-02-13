using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chocolatey.Gui.Base;
using Chocolatey.Gui.Controls;
using Chocolatey.Gui.Models;

namespace Chocolatey.Gui.Services
{
    public class ProgressService : ObservableBase, IProgressService
    {
        public ProgressService()
        {
            _isLoading = false;
            _loadingItems = 0;

            _output = new ObservableRingBuffer<PowerShellOutputLine>(100);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
        }

        private int _loadingItems;
        public void StartLoading()
        {
            lock (this)
            {
                _loadingItems++;
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
                _loadingItems--;
                if (_isLoading && _loadingItems < 1)
                {
                    _isLoading = false;
                    _output.Clear();
                    NotifyPropertyChanged("IsLoading");
                }
            }
        }

        private ObservableRingBuffer<PowerShellOutputLine> _output;
        public ObservableRingBuffer<PowerShellOutputLine> Output
        {
            get { return _output; }
        }
    }
}
