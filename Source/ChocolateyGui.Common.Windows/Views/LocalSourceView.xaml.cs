// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalSourceView.xaml.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Windows;
using System.Windows.Controls;
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

        public IList SelectedItems { get; private set; }

        /// <summary>
        /// This is an event handler for on selection change made on the list of the local packages, it will have the list of selected packages items which can be used
        /// later on to pass to the command.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventParameter</param>
        private void DgPackages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItems = ((System.Windows.Controls.Primitives.MultiSelector) e.OriginalSource).SelectedItems;
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