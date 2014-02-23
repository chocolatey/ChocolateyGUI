using System;
using System.ComponentModel;
using Chocolatey.Gui.Controls;
using Chocolatey.Gui.Models;

namespace Chocolatey.Gui.Services
{
    public interface IProgressService : INotifyPropertyChanged, IProgress<int>
    {
        bool IsLoading { get; }
        void StartLoading();
        void StopLoading();

        ObservableRingBuffer<PowerShellOutputLine> Output { get; }
    }
}
