// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="FauxPowerShellConsole.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Controls
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Threading;
    using ChocolateyGui.Models;

    /// <summary>
    /// Fake PowerShell Output Console
    /// </summary>
    public class FauxPowerShellConsole : RichTextBox
    {       
        /// <summary>
        /// The input buffer the console writes to the text box
        /// </summary>
        public static readonly DependencyProperty BufferCollectionProperty = DependencyProperty.Register(
            "BufferCollectionCollection",
            typeof(ObservableRingBufferCollection<PowerShellOutputLine>),
            typeof(FauxPowerShellConsole),
            new FrameworkPropertyMetadata
                {
                    DefaultValue = null,
                    PropertyChangedCallback = OnBufferChanged,
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

        /// <summary>
        /// The container paragraph for all the console's text.
        /// </summary>
        private readonly Paragraph _backingParagraph;

        /// <summary>
        /// Creates a unique identifier for each text line.
        /// </summary>
        private readonly Func<string, string> _getNameHash;

        public FauxPowerShellConsole()
            : base(new FlowDocument())
        {
            var hashAlg = MD5.Create();
            this._getNameHash = (unhashed) => "_" + hashAlg.ComputeHash(Encoding.UTF8.GetBytes(unhashed)).Aggregate(new StringBuilder(), (sb, piece) => sb.Append(piece.ToString("X2", CultureInfo.CurrentCulture))).ToString();

            this._backingParagraph = new Paragraph();
            Document.Blocks.Add(this._backingParagraph);
        }

        private delegate void RunStringOnUI(PowerShellOutputLine line);

        public ObservableRingBufferCollection<PowerShellOutputLine> BufferCollection
        {
            get { return this.GetValue<ObservableRingBufferCollection<PowerShellOutputLine>>(BufferCollectionProperty); }
            set { this.SetValue(BufferCollectionProperty, value); }
        }

        protected T GetValue<T>(DependencyProperty dependencyProperty)
        {
            return (T)this.GetValue(dependencyProperty);
        }

        protected void OnBufferUpdated(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    Application.Current.Dispatcher.InvokeAsync(() => this._backingParagraph.Inlines.Clear());
                    break;
                case NotifyCollectionChangedAction.Add:
                    foreach (PowerShellOutputLine item in args.NewItems)
                    {
                        Dispatcher.BeginInvoke(
                            new RunStringOnUI(
                                line =>
                                    {
                                var run = new Run
                                              {
                                                  Text = item.Text,
                                                  Name = _getNameHash(line.Text),
                                                  Foreground =
                                                      line.LineType == PowerShellLineType.Output
                                                          ? Brushes.White
                                                          : Brushes.Red,
                                                  Background =
                                                      line.LineType == PowerShellLineType.Output
                                                          ? Brushes.Transparent
                                                          : Brushes.Black
                                              };

                                if (item.NewLine)
                                {
                                    run.Text += Environment.NewLine;
                                }

                                this._backingParagraph.Inlines.Add(run);
                                Selection.Select(run.ContentStart, run.ContentEnd);
                            }), 
                            item);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (PowerShellOutputLine item in args.OldItems)
                    {
                        Dispatcher.BeginInvoke(
                            new RunStringOnUI(
                                line =>
                                {
                                    var key = this._getNameHash(line.Text);
                                    var run = this._backingParagraph.Inlines.FirstOrDefault(inline => inline.Name == key);
                                    if (run != null)
                                    {
                                        this._backingParagraph.Inlines.Remove(run);
                                    }
                                }),
                            item);
                    }

                    break;
            }
        }

        private static void OnBufferChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var pob = d as FauxPowerShellConsole;
            if (pob != null)
            {
                pob.OnBufferChanged(args);
            }
        }

        private void OnBufferChanged(DependencyPropertyChangedEventArgs args)
        {
            // If we had a previous buffer, clear our event holder.
            if (args.OldValue != null)
            {
                ((ObservableRingBufferCollection<PowerShellOutputLine>)args.OldValue).CollectionChanged -= this.OnBufferUpdated;
            }

            var newBuffer = (ObservableRingBufferCollection<PowerShellOutputLine>)args.NewValue;
            newBuffer.CollectionChanged += this.OnBufferUpdated;
            
            // Reset the current console.
            this.OnBufferUpdated(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            var bufferItems = newBuffer.ToList();

            // Add in any lines written to the buffer.
            this.OnBufferUpdated(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, bufferItems));
        }
    }
}