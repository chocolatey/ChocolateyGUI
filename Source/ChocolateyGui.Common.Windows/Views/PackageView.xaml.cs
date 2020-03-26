// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageView.xaml.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace ChocolateyGui.Common.Windows.Views
{
    /// <summary>
    ///     Interaction logic for PackageView.xaml
    /// </summary>
    public partial class PackageView
    {
        public PackageView()
        {
            InitializeComponent();
        }

        private void HandleLinkClick(object sender, RoutedEventArgs e)
        {
            var hl = (Hyperlink)sender;
            var navigateUri = hl.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
            e.Handled = true;
        }

        private void HandleMarkdownLink(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Parameter.ToString()));
        }
    }
}