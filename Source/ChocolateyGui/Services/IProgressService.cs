﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IProgressService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using ChocolateyGui.Controls;
using ChocolateyGui.Models;
using MahApps.Metro.Controls.Dialogs;

namespace ChocolateyGui.Services
{
    public interface IProgressService : INotifyPropertyChanged, IProgress<double>
    {
        bool IsLoading { get; }

        ObservableRingBufferCollection<PowerShellOutputLine> Output { get; }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not appropriate")]
        CancellationToken GetCancellationToken();

        Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null);

        Task StartLoading(string title = null, bool isCancelable = false);

        Task StopLoading();

        void WriteMessage(string message, PowerShellLineType type = PowerShellLineType.Output, bool newLine = true);
    }
}