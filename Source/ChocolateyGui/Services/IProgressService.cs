using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ChocolateyGui.Controls;
using ChocolateyGui.Models;
using MahApps.Metro.Controls.Dialogs;
using System.Threading;

namespace ChocolateyGui.Services
{
    public interface IProgressService : INotifyPropertyChanged, IProgress<double>
    {
        bool IsLoading { get; }
        Task StartLoading(string title = null, bool isCancelable = false);
        Task StopLoading();
        CancellationToken GetCancellationToken();

        ObservableRingBuffer<PowerShellOutputLine> Output { get; }

        void WriteMessage(string message, PowerShellLineType type = PowerShellLineType.Output, bool newLine = true);

        Task<MessageDialogResult> ShowMessageAsync(string title, string message);
    }
}
