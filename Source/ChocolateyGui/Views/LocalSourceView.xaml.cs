// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalSourceView.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Input;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.Views
{
    /// <summary>
    ///     Interaction logic for LocalSourceView.xaml
    /// </summary>
    public partial class LocalSourceView
    {
        public LocalSourceView()
        {
            InitializeComponent();
        }

        private void PackageDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dynamic source = e.OriginalSource;
            var item = source.DataContext as IPackageViewModel;
            item?.ViewDetails();
        }
    }
}