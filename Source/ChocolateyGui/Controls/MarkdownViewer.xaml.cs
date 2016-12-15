// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MarkdownViewer.xaml.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using CefSharp;
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
            Browser.RequestHandler = new ChocoRequestHandler();
            Browser.IsBrowserInitializedChanged += FirstStart;

            // Bind Properties
            this.ToObservable(MarkdownStringProperty, () => MarkdownString)
                .Subscribe(LoadMarkdown);

            Unloaded += MarkdownViewer_Unloaded;
        }

        public static readonly DependencyProperty MarkdownStringProperty = DependencyProperty.Register(
            "MarkdownString", typeof(string), typeof(MarkdownViewer), new PropertyMetadata(default(string)));

        public string MarkdownString
        {
            get { return (string)GetValue(MarkdownStringProperty); }
            set { SetValue(MarkdownStringProperty, value); }
        }

        public void FirstStart(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                InitializedBrowser();
            }
        }

        private void InitializedBrowser()
        {
            LoadMarkdown(MarkdownString);
        }

        private void LoadMarkdown(string markdown)
        {
            if (!Browser.IsInitialized)
            {
                return;
            }

            var newHtml = Markdown.ToHtml(markdown ?? string.Empty);
            var displayHtml = HtmlTemplate.Replace("{{content}}", newHtml);
            var url = $"http://rawhtml/{newHtml.GetHashCode()}";
            Browser.LoadHtml(displayHtml, url);
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
            if (!Browser.IsDisposed)
            {
                Browser.Dispose();
            }
        }
    }
}
