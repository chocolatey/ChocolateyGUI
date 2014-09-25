// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourcesControl.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Views.Controls
{
    using ChocolateyGui.ViewModels.Controls;

    /// <summary>
    /// Interaction logic for SourcesControl.xaml
    /// </summary>
    public partial class SourcesControl
    {
        public SourcesControl(ISourcesControlViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}