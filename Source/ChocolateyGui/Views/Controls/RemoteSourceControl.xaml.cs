// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="RemoteSourceControl.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Views.Controls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;
    using ChocolateyGui.Services;
    using ChocolateyGui.ViewModels.Controls;
    using ChocolateyGui.ViewModels.Items;
    using ChocoPM.Extensions;

    /// <summary>
    /// Interaction logic for RemoteSourceControl.xaml
    /// </summary>
    public partial class RemoteSourceControl
    {
        public const string PageTitle = "Remote Packages";

        private readonly Lazy<INavigationService> _navigationService;
        private readonly IRemoteSourceControlViewModel _viewModel;

        public RemoteSourceControl(IRemoteSourceControlViewModel viewModel, Lazy<INavigationService> navigationService)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }

            InitializeComponent();
            DataContext = viewModel;
            this._viewModel = viewModel;

            this._navigationService = navigationService;

            Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(viewModel.Packages, "CollectionChanged")
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Distinct()
                .ObserveOnDispatcher()
                .Subscribe(ev => this.Packages_CollectionChanged());
        }

        private void Packages_CollectionChanged()
        {
            // When our collection is updated, reset the DataTable's sort descriptions.
            PackagesGrid.Items.SortDescriptions.Clear();
            if (!string.IsNullOrWhiteSpace(this._viewModel.SortColumn))
            {
                PackagesGrid.Items.SortDescriptions.Add(
                    new SortDescription(
                        this._viewModel.SortColumn,
                        this._viewModel.SortDescending ? ListSortDirection.Descending : ListSortDirection.Ascending));
            }

            foreach (var column in PackagesGrid.Columns)
            {
                if (column.GetSortMemberPath() == this._viewModel.SortColumn)
                {
                    column.SortDirection = this._viewModel.SortDescending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }
                else
                {
                    column.SortDirection = null;
                }
            }
        }

        private void PackageDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dynamic source = e.OriginalSource;
            var item = source.DataContext as IPackageViewModel;
            if (item == null)
            {
                return;
            }

            this._navigationService.Value.Navigate(typeof(PackageControl), item);
        }

        private void DataGrid_OnSorting(object sender, DataGridSortingEventArgs e)
        {
            var sortPropertyName = e.Column.GetSortMemberPath();
            if (string.IsNullOrEmpty(sortPropertyName))
            {
                return;
            }

            bool sortDescending;
            if (!e.Column.SortDirection.HasValue || (e.Column.SortDirection.Value == ListSortDirection.Ascending))
            {
                sortDescending = true;
            }
            else
            {
                sortDescending = false;
            }

            this._viewModel.SortDescending = sortDescending;
            this._viewModel.SortColumn = sortPropertyName;
            e.Handled = true;
        }
    }
}