using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using ChocolateyGui.Services;
using ChocolateyGui.ViewModels.Controls;
using ChocolateyGui.ViewModels.Items;
using ChocoPM.Extensions;

namespace ChocolateyGui.Views.Controls
{
    public partial class RemoteSourceControl
    {
        public const string PageTitle = "Remote Packages";

        private readonly Lazy<INavigationService> _navigationService;
        private readonly IRemoteSourceControlViewModel _viewModel;

        public RemoteSourceControl(IRemoteSourceControlViewModel vm, Lazy<INavigationService> navigationService)
        {
            InitializeComponent();
            DataContext = vm;
            _viewModel = vm;

            _navigationService = navigationService;

            Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(vm.Packages, "CollectionChanged")
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Distinct()
                .ObserveOnDispatcher()
                .Subscribe(ev => Packages_CollectionChanged());
        }

        void Packages_CollectionChanged()
        {
            // When our collection is updated, reset the DataTable's sort descriptions.
            PackagesGrid.Items.SortDescriptions.Clear();
            if (!string.IsNullOrWhiteSpace(_viewModel.SortColumn))
                PackagesGrid.Items.SortDescriptions.Add(new SortDescription(_viewModel.SortColumn, _viewModel.SortDescending ? ListSortDirection.Descending : ListSortDirection.Ascending));

            foreach (var column in PackagesGrid.Columns)
            {
                if (column.GetSortMemberPath() == _viewModel.SortColumn)
                {
                    column.SortDirection = _viewModel.SortDescending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }
                else
                    column.SortDirection = null;
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

            _navigationService.Value.Navigate(typeof(PackageControl), item);
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
            _viewModel.SortDescending = sortDescending;
            _viewModel.SortColumn = sortPropertyName;
            e.Handled = true;
        }
    }
}
