using ChocolateyGui.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ChocolateyGui.Controls.Dialogs
{
    /// <summary>
    /// Interaction logic for ChocolateyDialog.xaml
    /// </summary>
    public partial class ChocolateyDialog : BaseMetroDialog
    {
        internal ChocolateyDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();

            if (parentWindow.MetroDialogOptions.ColorScheme == MetroDialogColorScheme.Theme)
            {
                try
                {
                    ProgressBarForeground = this.FindResource("AccentColorBrush") as Brush;
                }
                catch (Exception) { }
            }
            else
                ProgressBarForeground = Brushes.White;
        }
        internal ChocolateyDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
            
        }

        public static readonly DependencyProperty ProgressBarForegroundProperty = DependencyProperty.Register("ProgressBarForeground", typeof(Brush), typeof(ChocolateyDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty OutputBufferProperty = DependencyProperty.Register("OutputBuffer", typeof(ObservableRingBuffer<PowerShellOutputLine>), typeof(ChocolateyDialog),
            new PropertyMetadata(default(ObservableRingBuffer<PowerShellOutputLine>),
            new PropertyChangedCallback((s, e) =>
            {
                ((ChocolateyDialog)s).PART_Console.Buffer = (ObservableRingBuffer<PowerShellOutputLine>)e.NewValue;
            })));
        public static readonly DependencyProperty IsCancelableProperty = DependencyProperty.Register("IsCancelable", typeof(bool), typeof(ChocolateyDialog), new PropertyMetadata(default(bool), new PropertyChangedCallback((s, e) =>
        {
            ((ChocolateyDialog)s).PART_NegativeButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        })));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(ChocolateyDialog), new PropertyMetadata("Cancel"));

        public ObservableRingBuffer<PowerShellOutputLine> OutputBuffer
        {
            get { return (ObservableRingBuffer<PowerShellOutputLine>)GetValue(OutputBufferProperty); }
            set { SetValue(OutputBufferProperty, value); }
        }

        public bool IsCancelable
        {
            get { return (bool)GetValue(IsCancelableProperty); }
            set { SetValue(IsCancelableProperty, value); }
        }

        public string NegativeButtonText
        {
            get { return (string)GetValue(NegativeButtonTextProperty); }
            set { SetValue(NegativeButtonTextProperty, value); }
        }

        public Brush ProgressBarForeground
        {
            get { return (Brush)GetValue(ProgressBarForegroundProperty); }
            set { SetValue(ProgressBarForegroundProperty, value); }
        }
    }

    /// <summary>
    /// A class for manipulating an open ChocolateyDialog.
    /// </summary>
    public class ChocolateyDialogController
    {
        private ChocolateyDialog WrappedDialog { get; set; }

        private Func<Task> CloseCallback { get; set; }

        /// <summary>
        /// Gets if the wrapped ProgressDialog is open.
        /// </summary>
        public bool IsOpen { get; private set; }

        public delegate void DialogCancelledEventHandler(BaseMetroDialog dialog);

        public event DialogCancelledEventHandler OnCancelled;

        internal ChocolateyDialogController(ChocolateyDialog dialog, Func<Task> closeCallBack)
        {
            WrappedDialog = dialog;
            CloseCallback = closeCallBack;

            IsOpen = dialog.IsVisible;

            WrappedDialog.PART_NegativeButton.Dispatcher.Invoke(new Action(() =>
            {
                WrappedDialog.PART_NegativeButton.Click += PART_NegativeButton_Click;
            }));
        }

        void PART_NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                IsCanceled = true;
                WrappedDialog.PART_NegativeButton.IsEnabled = false;
                var cancelled = OnCancelled;
                if (cancelled != null)
                    cancelled(WrappedDialog);
            }
            else
            {
                WrappedDialog.Dispatcher.Invoke(new Action(() =>
                {
                    IsCanceled = true;
                    WrappedDialog.PART_NegativeButton.IsEnabled = false;
                    var cancelled = OnCancelled;
                    if (cancelled != null)
                        cancelled(WrappedDialog);
                }));
            }

            //Close();
        }

        /// <summary>
        /// Sets the ProgressBar's IsIndeterminate to true. To set it to false, call SetProgress.
        /// </summary>
        public void SetIndeterminate()
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
                WrappedDialog.PART_ProgressBar.IsIndeterminate = true;
            else
            {
                WrappedDialog.Dispatcher.Invoke(new Action(() =>
                                                {
                                                    WrappedDialog.PART_ProgressBar.IsIndeterminate = true;
                                                }));
            }
        }

        /// <summary>
        /// Sets if the Cancel button is visible.
        /// </summary>
        /// <param name="value"></param>
        public void SetCancelable(bool value)
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
                WrappedDialog.IsCancelable = value;
            else
                WrappedDialog.Dispatcher.Invoke(new Action(() =>
                {
                    WrappedDialog.IsCancelable = value;
                }));
        }

        /// <summary>
        /// Sets the dialog's progress bar value and sets IsIndeterminate to false.
        /// </summary>
        /// <param name="value">The percentage to set as the value.</param>
        public void SetProgress(double value)
        {
            if (value < 0.0 || value > 1.0) throw new ArgumentOutOfRangeException("value");

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
        /// Sets the dialog's title content.
        /// </summary>
        /// <param name="message">The title to be set.</param>
        public void SetTitle(string title)
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                WrappedDialog.Title = title;
            }
            else
            {
                WrappedDialog.Dispatcher.Invoke(new Action(() => { WrappedDialog.Title = title; }));
            }
        }

        /// <summary>
        /// Gets if the Cancel button has been pressed.
        /// </summary>
        public bool IsCanceled { get; private set; }

        /// <summary>
        /// Begins an operation to close the ProgressDialog.
        /// </summary>
        /// <returns>A task representing the operation.</returns>
        public async Task CloseAsync()
        {
            Action action = () =>
            {
                if (!IsOpen) throw new InvalidOperationException();
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

            WrappedDialog.Dispatcher.Invoke(new Action(() =>
            {
                IsOpen = false;
            }));
            return;
        }
    }

}
