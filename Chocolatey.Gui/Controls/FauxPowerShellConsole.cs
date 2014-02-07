using Chocolatey.Gui.Models;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Chocolatey.Gui.Controls
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

        private ObservableRingBuffer<PowerShellOutputLine> _oldBuffer;
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
            if (_oldBuffer != null)
                _oldBuffer.CollectionChanged -= OnBufferUpdated;

            _oldBuffer = Buffer;
            Buffer.CollectionChanged += OnBufferUpdated;
            OnBufferUpdated(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        private delegate void RunOnUI();
        private delegate void RunStringOnUI(PowerShellOutputLine line);

        protected void OnBufferUpdated(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    Application.Current.Dispatcher.Invoke(new RunOnUI(() => _backingParagraph.Inlines.Clear()));
                    break;
                case NotifyCollectionChangedAction.Add:
                    if (args.NewItems.Count == 1 && args.NewStartingIndex > 0)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new RunStringOnUI(item =>
                        {
                            var run = new Run
                            {
                                Text = item.Text + Environment.NewLine,
                                Name = _getNameHash(item.Text),
                                Foreground = item.Type == PowerShellLineType.Output ? Brushes.White : Brushes.Red,
                                Background = item.Type == PowerShellLineType.Output ? Brushes.Transparent : Brushes.Black
                            };

                            var beforeString = Buffer[args.NewStartingIndex - 1].Text;
                            var key = _getNameHash(beforeString);
                            var beforeRun = _backingParagraph.Inlines.FirstOrDefault(inline => inline.Name == key);
                            if (beforeRun != null)
                                _backingParagraph.Inlines.InsertAfter(beforeRun, run);
                            else
                                _backingParagraph.Inlines.Add(run);

                            Selection.Select(run.ContentStart, run.ContentEnd);
                        }), args.NewItems[0]);
                    }
                    foreach (PowerShellOutputLine item in args.NewItems)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new RunStringOnUI(line =>
                        {
                            var run = new Run
                            {
                                Text = line.Text + Environment.NewLine,
                                Name = _getNameHash(line.Text),
                                Foreground = line.Type == PowerShellLineType.Output ? Brushes.White : Brushes.Red,
                                Background = line.Type == PowerShellLineType.Output ? Brushes.Transparent : Brushes.Black
                            };

                            _backingParagraph.Inlines.Add(run);
                            Selection.Select(run.ContentStart, run.ContentEnd);
                        }), item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (PowerShellOutputLine item in args.OldItems)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new RunStringOnUI(line =>
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
