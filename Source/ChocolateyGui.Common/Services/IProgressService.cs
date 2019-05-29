// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IProgressService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using ChocolateyGui.Common.Controls;
using ChocolateyGui.Common.Models;
using MahApps.Metro.Controls.Dialogs;

namespace ChocolateyGui.Common.Services
{
    public interface IProgressService : INotifyPropertyChanged, IProgress<double>
    {
        bool IsLoading { get; }

        ObservableRingBufferCollection<PowerShellOutputLine> Output { get; }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not appropriate")]
        CancellationToken GetCancellationToken();

        Task<MessageDialogResult> ShowMessageAsync(string title, string message);

        Task StartLoading(string title = null, bool isCancelable = false);

        Task StopLoading();

        void WriteMessage(string message, PowerShellLineType type = PowerShellLineType.Output, bool newLine = true);
    }
}