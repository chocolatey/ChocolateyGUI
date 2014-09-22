// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IProgressService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using ChocolateyGui.Controls;
    using ChocolateyGui.Models;
    using MahApps.Metro.Controls.Dialogs;
   
    public interface IProgressService : INotifyPropertyChanged, IProgress<double>
    {
        bool IsLoading { get; }

        ObservableRingBuffer<PowerShellOutputLine> Output { get; }

        CancellationToken GetCancellationToken();

        Task<MessageDialogResult> ShowMessageAsync(string title, string message);

        Task StartLoading(string title = null, bool isCancelable = false);

        Task StopLoading();

        void WriteMessage(string message, PowerShellLineType type = PowerShellLineType.Output, bool newLine = true);
    }
}