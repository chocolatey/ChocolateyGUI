// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MarkdownViewer.xaml.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CefSharp;
using CefSharp.Wpf;
using ChocolateyGui.Providers;
using ChocolateyGui.Utilities.Extensions;
using Markdig;

namespace ChocolateyGui.Controls
{
    /// <summary>
    /// Interaction logic for MarkdownViewer.xaml
    /// </summary>
    public partial class MarkdownViewer
    {
        public static readonly DependencyProperty MarkdownSourceProperty = DependencyProperty.Register(
            "MarkdownSource",
            typeof(Uri),
            typeof(MarkdownViewer),
            new PropertyMetadata(default(Uri)));

        public static readonly DependencyProperty MarkdownStringProperty = DependencyProperty.Register(
            "MarkdownString",
            typeof(string),
            typeof(MarkdownViewer),
            new PropertyMetadata(default(string)));

        internal static readonly Lazy<string> HtmlTemplate = new Lazy<string>(() =>
$@"
<!DOCTYPE html>
<html>
    <head>
        <meta charset=""utf-8"">
        <title>Markdown</title>
        <style>
            {DefaultCss.Value}
        </style>
    </head>
    <body>
        {{{{content}}}}
    </body>
</html>
");

        internal static readonly Lazy<string> DefaultCss = new Lazy<string>(() =>
        {
            var assembly = typeof(MarkdownViewer).Assembly;
            var resource = assembly.GetManifestResourceStream(typeof(MarkdownViewer), "DefaultMarkdownStyle.css");
            Debug.Assert(resource != null, nameof(resource) + " != null");
            using (var streamReader = new StreamReader(resource))
            {
                return streamReader.ReadToEnd();
            }
        });

        private static readonly MarkdownPipeline DefaultPipeline =
            new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

        private static readonly DispatcherTimer DispatcherTimer;

        private static readonly ConcurrentBag<ChromiumWebBrowser> BrowserCleanupQueue =
            new ConcurrentBag<ChromiumWebBrowser>();

        private ChromiumWebBrowser _browser;

        static MarkdownViewer()
        {
            DispatcherTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            DispatcherTimer.Tick += DispatcherWork;
            DispatcherTimer.Start();
        }

        public MarkdownViewer()
        {
            InitializeComponent();

            // Setup Browser
            if (DesignMode.IsInDesignModeStatic)
            {
                SetBrowser();
            }

            // Bind Properties
            this.ToObservable(MarkdownStringProperty, () => MarkdownString)
                .Subscribe(LoadMarkdown);

            Unloaded += OnUnloaded;
            Loaded += OnLoaded;
        }

        public string MarkdownString
        {
            get { return (string)GetValue(MarkdownStringProperty); }
            set { SetValue(MarkdownStringProperty, value); }
        }

        public Uri MarkdownSource
        {
            get
            {
                return (Uri)GetValue(MarkdownSourceProperty);
            }

            set
            {
                SetValue(MarkdownSourceProperty, value);
            }
        }

        public void FirstStart(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                InitializedBrowser();
            }
        }

        private static void DispatcherWork(object sender, EventArgs e)
        {
            ChromiumWebBrowser browser;
            while (BrowserCleanupQueue.TryTake(out browser))
            {
                browser.Dispose();
            }
        }

        private void SetBrowser()
        {
            _browser = new ChromiumWebBrowser { RequestHandler = new ChocoRequestHandler() };
            _browser.IsBrowserInitializedChanged += FirstStart;
            PART_BrowserHost.Content = _browser;
            RenderOptions.SetBitmapScalingMode(_browser, BitmapScalingMode.NearestNeighbor);
            PART_BrowserHost.UseLayoutRounding = true;
        }

        private void InitializedBrowser()
        {
            if (MarkdownSource != null)
            {
                var resourceInfo = Application.GetResourceStream(MarkdownSource);
                if (resourceInfo == null)
                {
                    throw new InvalidOperationException($"Failed to find markdown resource \"{MarkdownSource}\".");
                }

                using (var stream = resourceInfo.Stream)
                using (var reader = new StreamReader(stream))
                {
                    LoadMarkdown(reader.ReadToEnd());
                }
            }
            else
            {
                LoadMarkdown(MarkdownString);
            }
        }

        private void LoadMarkdown(string markdown)
        {
            if (_browser == null || !_browser.IsInitialized)
            {
                return;
            }

            var newHtml = Markdown.ToHtml(markdown ?? string.Empty, DefaultPipeline);
            var displayHtml = HtmlTemplate.Value.Replace("{{content}}", newHtml);
            var url = $"http://rawhtml/{newHtml.GetHashCode()}";
            _browser.LoadHtml(displayHtml, url);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_browser?.IsDisposed == false)
            {
                BrowserCleanupQueue.Add(_browser);
                _browser = null;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_browser == null)
            {
                SetBrowser();
            }
        }
    }
}