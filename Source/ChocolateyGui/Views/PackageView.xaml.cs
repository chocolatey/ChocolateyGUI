// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageView.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace ChocolateyGui.Views
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
    }
}