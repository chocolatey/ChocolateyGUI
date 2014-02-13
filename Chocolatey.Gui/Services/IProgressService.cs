using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Chocolatey.Gui.Controls;
using Chocolatey.Gui.Models;

namespace Chocolatey.Gui.Services
{
    public interface IProgressService : INotifyPropertyChanged
    {
        bool IsLoading { get; }
        void StartLoading();
        void StopLoading();

        ObservableRingBuffer<PowerShellOutputLine> Output { get; }
    }
}
