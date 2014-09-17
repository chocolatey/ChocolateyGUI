using ChocolateyGui.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace ChocolateyGui.Controls
{
    /// <summary>
    /// Fake PowerShell Output Console
    /// </summary>
    public class FauxPowerShellConsole : RichTextBox
    {       
        /// <summary>
        /// The input buffer the console writes to the text box
        /// </summary>
        public static readonly DependencyProperty BufferProperty = DependencyProperty.Register("Buffer", typeof(ObservableRingBuffer<PowerShellOutputLine>), typeof(FauxPowerShellConsole),
            new FrameworkPropertyMetadata { DefaultValue = null, PropertyChangedCallback = OnBufferChanged, BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

        public ObservableRingBuffer<PowerShellOutputLine> Buffer
        {
            get { return GetValue<ObservableRingBuffer<PowerShellOutputLine>>(BufferProperty); }
            set { SetValue(BufferProperty, value); }
        }

        /// <summary>
        /// Creates a unique identifier for each text line.
        /// </summary>
        private readonly Func<string, string> _getNameHash;

        /// <summary>
        /// The container paragraph for all the console's text.
        /// </summary>
        private readonly Paragraph _backingParagraph;

        private readonly Dispatcher _windowDispatcher;

        public FauxPowerShellConsole() : base(new FlowDocument())
        {
            var hashAlg = MD5.Create();
            _getNameHash = (unhashed) => "_" + hashAlg.ComputeHash(Encoding.UTF8.GetBytes(unhashed)).Aggregate(new StringBuilder(), (sb, piece) => sb.Append(piece.ToString("X2"))).ToString();

            _backingParagraph = new Paragraph();
            Document.Blocks.Add(_backingParagraph);
        }


        private static void OnBufferChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var pob = d as FauxPowerShellConsole;
            if (pob != null)
                pob.OnBufferChanged(args);
        }

        private void OnBufferChanged(DependencyPropertyChangedEventArgs args)
        {
            // If we had a previous buffer, clear our event holder.
            if (args.OldValue != null)
                ((ObservableRingBuffer<PowerShellOutputLine>)args.OldValue).CollectionChanged -= OnBufferUpdated;

            var newBuffer = (ObservableRingBuffer<PowerShellOutputLine>)args.NewValue;
            newBuffer.CollectionChanged += OnBufferUpdated;
            
            // Reset the current console.
            OnBufferUpdated(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            var bufferItems = newBuffer.ToList();

            // Add in any lines written to the buffer.
            OnBufferUpdated(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, bufferItems));
        }


        private delegate void RunOnUI();
        private delegate void RunStringOnUI(PowerShellOutputLine line);

        protected void OnBufferUpdated(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    Application.Current.Dispatcher.BeginInvoke(new RunOnUI(() => _backingParagraph.Inlines.Clear()));
                    break;
                case NotifyCollectionChangedAction.Add:
                    foreach (PowerShellOutputLine item in args.NewItems)
                    {
                        Dispatcher.BeginInvoke(new RunStringOnUI(line =>
                        {
                            var run = new Run
                            {
                                Text = item.Text,
                                Name = _getNameHash(line.Text),
                                Foreground = line.Type == PowerShellLineType.Output ? Brushes.White : Brushes.Red,
                                Background = line.Type == PowerShellLineType.Output ? Brushes.Transparent : Brushes.Black
                            };

                            if (item.NewLine)
                                run.Text += Environment.NewLine;

                            _backingParagraph.Inlines.Add(run);
                            Selection.Select(run.ContentStart, run.ContentEnd);
                        }), item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (PowerShellOutputLine item in args.OldItems)
                    {
                        Dispatcher.BeginInvoke(new RunStringOnUI(line =>
                        {
                            var key = _getNameHash(line.Text);
                            var run = _backingParagraph.Inlines.FirstOrDefault(inline => inline.Name == key);
                            if (run != null)
                                _backingParagraph.Inlines.Remove(run);
                        }), item);
                    }
                    break;
            }
        }

        protected T GetValue<T>(DependencyProperty dp)
        {
            return (T)GetValue(dp);
        }
    }
}
