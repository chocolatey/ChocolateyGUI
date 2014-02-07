using System.Windows.Controls;
using Chocolatey.Gui.ViewModels.Pages;

namespace Chocolatey.Gui.Views.Pages
{
    /// <summary>
    /// Interaction logic for LocalSourcePage.xaml
    /// </summary>
    public partial class RemoteSourcePage : Page
    {
        public RemoteSourcePage(IRemoteSourcePageViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
