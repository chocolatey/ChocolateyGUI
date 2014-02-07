using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chocolatey.Gui.ViewModels.Pages;

namespace Chocolatey.Gui.Views.Pages
{
    /// <summary>
    /// Interaction logic for LocalSourcePage.xaml
    /// </summary>
    public partial class LocalSourcePage : Page
    {
        public LocalSourcePage(ILocalSourcePageViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
