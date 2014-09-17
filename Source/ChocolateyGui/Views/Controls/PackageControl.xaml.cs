using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ChocolateyGui.ViewModels.Controls;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.Views.Controls
{
    /// <summary>
    /// Interaction logic for PackageControl.xaml
    /// </summary>
    public partial class PackageControl
    {
        public PackageControl(IPackageControlViewModel vm, IPackageViewModel packageViewModel)
        {
            InitializeComponent();
            vm.Package = packageViewModel;
            DataContext = vm;
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
