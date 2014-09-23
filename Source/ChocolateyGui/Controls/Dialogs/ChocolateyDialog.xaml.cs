// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyDialog.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Controls.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using ChocolateyGui.Models;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    
    /// <summary>
    /// Interaction logic for ChocolateyDialog.xaml
    /// </summary>
    public partial class ChocolateyDialog : BaseMetroDialog
    {
        public static readonly DependencyProperty IsCancelableProperty = DependencyProperty.Register(
            "IsCancelable",
            typeof(bool),
            typeof(ChocolateyDialog),
            new PropertyMetadata(
                default(bool),
                new PropertyChangedCallback(
                    (s, e) =>
                        {
                            ((ChocolateyDialog)s).PART_NegativeButton.Visibility = (bool)e.NewValue
                                                                                       ? Visibility.Visible
                                                                                       : Visibility.Collapsed;
                        })));

        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(ChocolateyDialog), new PropertyMetadata("Cancel"));

        public static readonly DependencyProperty OutputBufferProperty = DependencyProperty.Register(
            "OutputBuffer",
            typeof(ObservableRingBuffer<PowerShellOutputLine>),
            typeof(ChocolateyDialog),
            new PropertyMetadata(
                default(ObservableRingBuffer<PowerShellOutputLine>),
                new PropertyChangedCallback((s, e) =>
            {
                ((ChocolateyDialog)s).PART_Console.Buffer = (ObservableRingBuffer<PowerShellOutputLine>)e.NewValue;
            })));

        public static readonly DependencyProperty ProgressBarForegroundProperty = DependencyProperty.Register("ProgressBarForeground", typeof(Brush), typeof(ChocolateyDialog), new PropertyMetadata(default(string)));

        internal ChocolateyDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            this.InitializeComponent();

            if (parentWindow.MetroDialogOptions.ColorScheme == MetroDialogColorScheme.Theme)
            {
                try
                {
                    this.ProgressBarForeground = this.FindResource("AccentColorBrush") as Brush;
                }
                catch (Exception)
                {
                }
            }
            else
            {
                this.ProgressBarForeground = Brushes.White;
            }
        }

        internal ChocolateyDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        public bool IsCancelable
        {
            get { return (bool)this.GetValue(IsCancelableProperty); }
            set { this.SetValue(IsCancelableProperty, value); }
        }

        public string NegativeButtonText
        {
            get { return (string)this.GetValue(NegativeButtonTextProperty); }
            set { this.SetValue(NegativeButtonTextProperty, value); }
        }

        public ObservableRingBuffer<PowerShellOutputLine> OutputBuffer
        {
            get { return (ObservableRingBuffer<PowerShellOutputLine>)this.GetValue(OutputBufferProperty); }
            set { this.SetValue(OutputBufferProperty, value); }
        }

        public Brush ProgressBarForeground
        {
            get { return (Brush)this.GetValue(ProgressBarForegroundProperty); }
            set { this.SetValue(ProgressBarForegroundProperty, value); }
        }
    }
}