// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyTaskCompletion.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ChocolateyGui.Common.Base;

namespace ChocolateyGui.Common.Windows.Utilities
{
    public sealed class NotifyTaskCompletion<TResult> : ObservableBase
    {
        private static readonly Serilog.ILogger Logger = Serilog.Log.ForContext<NotifyTaskCompletion<TResult>>();

        public NotifyTaskCompletion(Task<TResult> task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                var watchTask = WatchTaskAsync(task);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            }
        }

        public Task<TResult> Task { get; }

        public TResult Result
        {
            get { return Task.Status == TaskStatus.RanToCompletion ? Task.Result : default; }
        }

        public TaskStatus Status
        {
            get { return Task.Status; }
        }

        public bool IsCompleted
        {
            get { return Task.IsCompleted; }
        }

        public bool IsNotCompleted
        {
            get { return !Task.IsCompleted; }
        }

        public bool IsSuccessfullyCompleted
        {
            get
            {
                return Task.Status == TaskStatus.RanToCompletion;
            }
        }

        public bool IsCanceled
        {
            get { return Task.IsCanceled; }
        }

        public bool IsFaulted
        {
            get { return Task.IsFaulted; }
        }

        public AggregateException Exception
        {
            get { return Task.Exception; }
        }

        public Exception InnerException
        {
            get { return Exception?.InnerException; }
        }

        public string ErrorMessage
        {
            get { return InnerException?.Message; }
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ran into an error while executing a task.");
            }

            NotifyPropertyChanged(nameof(Status));
            NotifyPropertyChanged(nameof(IsCompleted));
            NotifyPropertyChanged(nameof(IsNotCompleted));

            if (task.IsCanceled)
            {
                NotifyPropertyChanged(nameof(IsCanceled));
            }
            else if (task.IsFaulted)
            {
                NotifyPropertyChanged(nameof(IsFaulted));
                NotifyPropertyChanged(nameof(Exception));
                NotifyPropertyChanged(nameof(InnerException));
                NotifyPropertyChanged(nameof(ErrorMessage));
            }
            else
            {
                NotifyPropertyChanged(nameof(IsSuccessfullyCompleted));
                NotifyPropertyChanged(nameof(Result));
            }

            CommandManager.InvalidateRequerySuggested();
        }
    }
}