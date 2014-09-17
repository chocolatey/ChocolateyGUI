using ChocolateyGui.ViewModels.Controls;

namespace ChocolateyGui.Views.Controls
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
