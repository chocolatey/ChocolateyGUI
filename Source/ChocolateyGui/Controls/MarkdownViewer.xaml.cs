// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MarkdownViewer.xaml.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
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

            Unloaded += MarkdownViewer_Unloaded;
            Loaded += OnLoaded;
        }

        public static readonly DependencyProperty MarkdownStringProperty = DependencyProperty.Register(
            "MarkdownString", typeof(string), typeof(MarkdownViewer), new PropertyMetadata(default(string)));

        public string MarkdownString
        {
            get { return (string)GetValue(MarkdownStringProperty); }
            set { SetValue(MarkdownStringProperty, value); }
        }

        public static readonly DependencyProperty MarkdownSourceProperty = DependencyProperty.Register("MarkdownSource", typeof(Uri), typeof(MarkdownViewer), new PropertyMetadata(default(Uri)));

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

        private ChromiumWebBrowser _browser;

        public void FirstStart(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                InitializedBrowser();
            }
        }

        private void SetBrowser()
        {
            _browser = new ChromiumWebBrowser { RequestHandler = new ChocoRequestHandler() };
            _browser.IsBrowserInitializedChanged += FirstStart;
            PART_BrowserHost.Content = _browser;
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

            var newHtml = Markdown.ToHtml(markdown ?? string.Empty);
            var displayHtml = HtmlTemplate.Replace("{{content}}", newHtml);
            var url = $"http://rawhtml/{newHtml.GetHashCode()}";
            _browser.LoadHtml(displayHtml, url);
        }

        internal const string HtmlTemplate = @"
<!doctype html>
<html class=""no-js"" lang="""">
    <head>
        <meta charset=""utf-8"">
        <meta http-equiv=""X-UA-Compatible"" content=""IE=edge,chrome=1"">
        <title>Markdown</title>
        <meta name=""description"" content="""">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
        <style>
            html, body {
                font-family: ""Segoe UI"", sans-serif;
                font-size: 14px;
                margin: 0;
                margin-top: -5px;
                padding: 0;
            }
        </style>
    </head>
    <body>
        {{content}}
    </body>
</html>
";

        private void MarkdownViewer_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_browser != null && !_browser.IsDisposed)
            {
                // Off load dispose, as it's an expensive blocking call.
                var browser = _browser;
                Execute.BeginOnUIThread(() => browser.Dispose());
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
