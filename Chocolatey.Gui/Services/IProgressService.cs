using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Chocolatey.Gui.Controls;
using Chocolatey.Gui.Models;
using MahApps.Metro.Controls.Dialogs;

namespace Chocolatey.Gui.Services
{
    public interface IProgressService : INotifyPropertyChanged, IProgress<int>
    {
        bool IsLoading { get; }
        void StartLoading(string title = "", string message = "");
        void StopLoading();

        ObservableRingBuffer<PowerShellOutputLine> Output { get; }

        Task<MessageDialogResult> ShowMessage(string title, string message);
    }
}
