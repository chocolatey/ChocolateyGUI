using Chocolatey.Gui.ViewModels.Controls;

namespace Chocolatey.Gui.Views.Controls
{
    /// <summary>
    /// Interaction logic for SourcesControl.xaml
    /// </summary>
    public partial class SourcesControl
    {
        public SourcesControl(ISourcesControlViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
