// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyDialogController.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Controls.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using MahApps.Metro.Controls.Dialogs;

    /// <summary>
    /// A class for manipulating an open ChocolateyDialog.
    /// </summary>
    public class ChocolateyDialogController
    {
        internal ChocolateyDialogController(ChocolateyDialog dialog, Func<Task> closeCallBack)
        {
            this.WrappedDialog = dialog;
            this.CloseCallback = closeCallBack;

            this.IsOpen = dialog.IsVisible;

            this.WrappedDialog.PART_NegativeButton.Dispatcher.Invoke(new Action(() =>
            {
                this.WrappedDialog.PART_NegativeButton.Click += PART_NegativeButton_Click;
            }));
        }

        public delegate void DialogCanceledEventHandler(BaseMetroDialog dialog);

        public event DialogCanceledEventHandler OnCanceled;

        /// <summary>
        /// Gets a value indicating whether the Cancel button has been pressed.
        /// </summary>
        public bool IsCanceled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the wrapped ProgressDialog is open.
        /// </summary>
        public bool IsOpen { get; private set; }

        private Func<Task> CloseCallback { get; set; }

        private ChocolateyDialog WrappedDialog { get; set; }

        /// <summary>
        /// Begins an operation to close the ProgressDialog.
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

            if (this.WrappedDialog.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                this.WrappedDialog.Dispatcher.Invoke(action);
            }

            await this.CloseCallback();

            this.WrappedDialog.Dispatcher.Invoke(new Action(() =>
            {
                IsOpen = false;
            }));
            return;
        }

        /// <summary>
        /// Sets if the Cancel button is visible.
        /// </summary>
        /// <param name="value">Default is false</param>
        public void SetCancelable(bool value)
        {
            if (this.WrappedDialog.Dispatcher.CheckAccess())
            {
                this.WrappedDialog.IsCancelable = value;
            }
            else
            {
                this.WrappedDialog.Dispatcher.Invoke(
                    new Action(
                        () =>
                        {
                            WrappedDialog.IsCancelable = value;
                        }));
            }
        }

        /// <summary>
        /// Sets the ProgressBar's IsIndeterminate to true. To set it to false, call SetProgress.
        /// </summary>
        public void SetIndeterminate()
        {
            if (this.WrappedDialog.Dispatcher.CheckAccess())
            {
                this.WrappedDialog.PART_ProgressBar.IsIndeterminate = true;
            }
            else
            {
                this.WrappedDialog.Dispatcher.Invoke(
                    new Action(
                        () =>
                        {
                            WrappedDialog.PART_ProgressBar.IsIndeterminate = true;
                        }));
            }
        }

        /// <summary>
        /// Sets the dialog's progress bar value and sets IsIndeterminate to false.
        /// </summary>
        /// <param name="value">The percentage to set as the value.</param>
        public void SetProgress(double value)
        {
            if (value < 0.0 || value > 1.0)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            Action action = () =>
            {
                WrappedDialog.PART_ProgressBar.IsIndeterminate = false;
                WrappedDialog.PART_ProgressBar.Value = value;
                WrappedDialog.PART_ProgressBar.Maximum = 1.0;
                WrappedDialog.PART_ProgressBar.ApplyTemplate();
            };

            if (this.WrappedDialog.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                this.WrappedDialog.Dispatcher.Invoke(action);
            }
        }

        /// <summary>
        /// Sets the dialog's title content.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void SetTitle(string title)
        {
            if (this.WrappedDialog.Dispatcher.CheckAccess())
            {
                this.WrappedDialog.Title = title;
            }
            else
            {
                this.WrappedDialog.Dispatcher.Invoke(new Action(() => { WrappedDialog.Title = title; }));
            }
        }

        private void PART_NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WrappedDialog.Dispatcher.CheckAccess())
            {
                this.IsCanceled = true;
                this.WrappedDialog.PART_NegativeButton.IsEnabled = false;
                var cancelled = this.OnCanceled;
                if (cancelled != null)
                {
                    cancelled(this.WrappedDialog);
                }
            }
            else
            {
                this.WrappedDialog.Dispatcher.Invoke(new Action(() =>
                {
                    IsCanceled = true;
                    this.WrappedDialog.PART_NegativeButton.IsEnabled = false;
                    var cancelled = this.OnCanceled;
                    if (cancelled != null)
                    {
                        cancelled(this.WrappedDialog);
                    }
                }));
            }

            // Close();
        }
    }
}