// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalSourceControl.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Views.Controls
{
    using System.Windows.Input;
    using ChocolateyGui.ViewModels.Controls;
    using ChocolateyGui.ViewModels.Items;

    /// <summary>
    /// Interaction logic for LocalSourceControl.xaml
    /// </summary>
    public partial class LocalSourceControl
    {
        public LocalSourceControl(ILocalSourceControlViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            Loaded += viewModel.Loaded;
        }

        private void PackageDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dynamic source = e.OriginalSource;
            var item = source.DataContext as IPackageViewModel;
            if (item == null)
            {
                return;
            }

            item.ViewDetails();
        }
    }
}
