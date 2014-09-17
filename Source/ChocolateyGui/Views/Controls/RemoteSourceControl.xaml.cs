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

namespace ChocolateyGui.Views.Controls
{
    public partial class RemoteSourceControl
    {
        public const string PageTitle = "Remote Packages";
        private readonly Lazy<INavigationService> _navigationService; 
        public RemoteSourceControl(IRemoteSourceControlViewModel vm, Lazy<INavigationService> navigationService)
        {
            InitializeComponent();
            DataContext = vm;

            _navigationService = navigationService;

            Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(vm.Packages, "CollectionChanged")
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Distinct()
                .ObserveOnDispatcher()
                .Subscribe(ev => Packages_CollectionChanged());
        }

        void Packages_CollectionChanged()
        {
            var _vm = DataContext as IRemoteSourceControlViewModel;
            PackagesGrid.Items.SortDescriptions.Clear();
            if (!string.IsNullOrWhiteSpace(_vm.SortColumn))
                PackagesGrid.Items.SortDescriptions.Add(new SortDescription(_vm.SortColumn, _vm.SortDescending ? ListSortDirection.Descending : ListSortDirection.Ascending));

            foreach (var column in PackagesGrid.Columns)
            {
                if (column.GetSortMemberPath() == _vm.SortColumn)
                {
                    column.SortDirection = _vm.SortDescending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }
                else
                    column.SortDirection = null;
            }
        }

        private void PackageDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dynamic source = e.OriginalSource;
            var item = source.DataContext as IPackageViewModel;
            if (item != null)
            {
                _navigationService.Value.Navigate(typeof(PackageControl), item);
            }
        }

        private void DataGrid_OnSorting(object sender, DataGridSortingEventArgs e)
        {
            var _vm = DataContext as IRemoteSourceControlViewModel;
            string sortPropertyName = e.Column.GetSortMemberPath();
            if (!string.IsNullOrEmpty(sortPropertyName))
            {
                bool sortDescending;
                if (!e.Column.SortDirection.HasValue || (e.Column.SortDirection.Value == ListSortDirection.Ascending))
                {
                    sortDescending = true;
                }
                else
                {
                    sortDescending = false;
                }
                _vm.SortDescending = sortDescending;
                _vm.SortColumn = sortPropertyName;
                e.Handled = true;
            }
        }
    }
}
