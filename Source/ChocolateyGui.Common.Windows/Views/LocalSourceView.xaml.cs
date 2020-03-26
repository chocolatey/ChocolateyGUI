// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalSourceView.xaml.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Input;
using ChocolateyGui.Common.ViewModels.Items;

namespace ChocolateyGui.Common.Windows.Views
{
    /// <summary>
    ///     Interaction logic for LocalSourceView.xaml
    /// </summary>
    public partial class LocalSourceView
    {
        public LocalSourceView()
        {
            InitializeComponent();

            PART_Loading.Margin = new Thickness(0, 0, 13, 0);

            this.Loaded += LocalSourceViewOnLoaded;
        }

        private void LocalSourceViewOnLoaded(object sender, RoutedEventArgs e)
        {
            this.SearchTextBox.Focus();
        }

        private void PackageDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dynamic source = e.OriginalSource;
            var item = source.DataContext as IPackageViewModel;
            item?.ViewDetails();
        }
    }
}