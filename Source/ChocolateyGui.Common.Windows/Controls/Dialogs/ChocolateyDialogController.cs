// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyDialogController.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace ChocolateyGui.Common.Windows.Controls.Dialogs
{
    /// <summary>
    ///     A class for manipulating an open ChocolateyDialog.
    /// </summary>
    public class ChocolateyDialogController
    {
        internal ChocolateyDialogController(ChocolateyDialog dialog, Func<Task> closeCallBack)
        {
            WrappedDialog = dialog;
            CloseCallback = closeCallBack;

            IsOpen = dialog.IsVisible;

            WrappedDialog.PART_NegativeButton.Dispatcher.Invoke(
                () => { WrappedDialog.PART_NegativeButton.Click += PART_NegativeButton_Click; });
        }

        public delegate void DialogCanceledEventHandler(BaseMetroDialog dialog);

        public event DialogCanceledEventHandler OnCanceled;

        /// <summary>
        ///     Gets a value indicating whether the Cancel button has been pressed.
        /// </summary>
        public bool IsCanceled { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the wrapped ProgressDialog is open.
        /// </summary>
        public bool IsOpen { get; private set; }

        private Func<Task> CloseCallback { get; }

        private ChocolateyDialog WrappedDialog { get; }

        /// <summary>
        ///     Begins an operation to close the ProgressDialog.
        /// </summary>
        /// <returns>A task representing the operation.</returns>
        public async Task CloseAsync()
        {
            Action action = () =>
            {
                if (!IsOpen)
                {
                    throw new InvalidOperationException();
                }

                WrappedDialog.Dispatcher.VerifyAccess();
                WrappedDialog.PART_NegativeButton.Click -= PART_NegativeButton_Click;
            };

            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                WrappedDialog.Dispatcher.Invoke(action);
            }

            await CloseCallback();

            WrappedDialog.Dispatcher.Invoke(() => { IsOpen = false; });
        }

        /// <summary>
        ///     Sets if the Cancel button is visible.
        /// </summary>
        /// <param name="value">Default is false</param>
        public void SetCancelable(bool value)
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                WrappedDialog.IsCancelable = value;
            }
            else
            {
                WrappedDialog.Dispatcher.Invoke(
                    () => { WrappedDialog.IsCancelable = value; });
            }
        }

        /// <summary>
        ///     Sets the ProgressBar's IsIndeterminate to true. To set it to false, call SetProgress.
        /// </summary>
        public void SetIndeterminate()
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                WrappedDialog.PART_ProgressBar.IsIndeterminate = true;
            }
            else
            {
                WrappedDialog.Dispatcher.Invoke(
                    () => { WrappedDialog.PART_ProgressBar.IsIndeterminate = true; });
            }
        }

        /// <summary>
        ///     Sets the dialog's progress bar value and sets IsIndeterminate to false.
        /// </summary>
        /// <param name="value">The percentage to set as the value.</param>
        public void SetProgress(double value)
        {
            if (value < 0.0 || value > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Action action = () =>
            {
                WrappedDialog.PART_ProgressBar.IsIndeterminate = false;
                WrappedDialog.PART_ProgressBar.Value = value;
                WrappedDialog.PART_ProgressBar.Maximum = 1.0;
                WrappedDialog.PART_ProgressBar.ApplyTemplate();
            };

            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                WrappedDialog.Dispatcher.Invoke(action);
            }
        }

        /// <summary>
        ///     Sets the dialog's title content.
        /// </summary>
        /// <param name="title">
        ///     The title.
        /// </param>
        public void SetTitle(string title)
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                WrappedDialog.Title = title;
            }
            else
            {
                WrappedDialog.Dispatcher.Invoke(() => { WrappedDialog.Title = title; });
            }
        }

        private void PART_NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                IsCanceled = true;
                WrappedDialog.PART_NegativeButton.IsEnabled = false;
            }
            else
            {
                WrappedDialog.Dispatcher.Invoke(() =>
                {
                    IsCanceled = true;
                    WrappedDialog.PART_NegativeButton.IsEnabled = false;
                    OnCanceled?.Invoke(WrappedDialog);
                });
            }

            // Close();
        }
    }
}