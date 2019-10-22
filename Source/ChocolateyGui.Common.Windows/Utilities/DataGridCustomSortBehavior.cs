// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="DataGridCustomSortBehavior.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Xaml.Behaviors;

namespace ChocolateyGui.Common.Windows.Utilities
{
    public class DataGridCustomSortBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty CustomComparerProperty
            = DependencyProperty.RegisterAttached("CustomComparer", typeof(IDataGridColumnComparer), typeof(DataGridCustomSortBehavior));

        public static IDataGridColumnComparer GetCustomComparer(DataGridColumn gridColumn)
        {
            return (IDataGridColumnComparer)gridColumn.GetValue(CustomComparerProperty);
        }

        public static void SetCustomComparer(DataGridColumn gridColumn, IDataGridColumnComparer value)
        {
            gridColumn.SetValue(CustomComparerProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Sorting += HandleCustomSorting;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Sorting -= HandleCustomSorting;

            base.OnDetaching();
        }

        private void HandleCustomSorting(object sender, DataGridSortingEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null)
            {
                return;
            }

            var listColView = dataGrid.ItemsSource as ListCollectionView;
            if (listColView == null)
            {
                // The DataGrid's ItemsSource property must be of type, ListCollectionView
                return;
            }

            var direction = (e.Column.SortDirection != ListSortDirection.Ascending)
                ? ListSortDirection.Ascending
                : ListSortDirection.Descending;

            // Get the custom sorter for this column
            var sorter = GetCustomComparer(e.Column);
            if (sorter == null)
            {
                if (listColView.CustomSort != null)
                {
                    e.Handled = true;
                    e.Column.SortDirection = direction;
                    listColView.CustomSort = null;
                }

                return;
            }

            // Yes, we handle it
            e.Handled = true;
            e.Column.SortDirection = sorter.SortDirection = direction;
            listColView.CustomSort = sorter;
        }
    }
}