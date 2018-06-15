// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AboutView.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChocolateyGui.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void HandleMarkdownLink(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Parameter.ToString()));
        }
    }
}