using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Chocolatey.Gui.ViewModels.Controls;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.Views.Controls
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
