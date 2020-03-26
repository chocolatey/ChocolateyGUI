// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MarkdownEx.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Windows;
using Markdig.Wpf;

namespace ChocolateyGui.Common.Windows.Controls
{
    public static class MarkdownEx
    {
        public static readonly DependencyProperty UrlProperty = DependencyProperty.RegisterAttached(
            "Url",
            typeof(string),
            typeof(MarkdownViewer),
            new PropertyMetadata(default(string), PropertyChangedCallback));

        public static void SetUrl(DependencyObject element, string value)
        {
            element.SetValue(UrlProperty, value);
        }

        public static string GetUrl(DependencyObject element)
        {
            return (string)element.GetValue(UrlProperty);
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var viewer = (MarkdownViewer)dependencyObject;
            var url = e.NewValue as string;
            if (string.IsNullOrWhiteSpace(url))
            {
                viewer.Markdown = null;
                return;
            }

            Uri uri;
            if (!url.StartsWith("pack:") || !Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                viewer.Markdown = null;
                return;
            }

            var resource = Application.GetResourceStream(uri);
            if (resource == null)
            {
                viewer.Markdown = null;
                return;
            }

            using (var reader = new StreamReader(resource.Stream))
            {
                viewer.Markdown = reader.ReadToEnd();
            }
        }
    }
}