using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Chocolatey.Gui.ViewModels.Items;
using Chocolatey.Gui.ViewModels.Pages;

namespace Chocolatey.Gui.Views.Pages
{
    /// <summary>
    /// Interaction logic for PackagePage.xaml
    /// </summary>
    public partial class PackagePage
    {
        public PackagePage(IPackagePageViewModel vm, IPackageViewModel packageViewModel)
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
