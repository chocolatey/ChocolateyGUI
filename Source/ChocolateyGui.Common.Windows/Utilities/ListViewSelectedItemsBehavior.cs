using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace ChocolateyGui.Common.Windows.Utilities
{
    public class ListViewSelectedItemsBehavior : Behavior<ListView>
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof(IList),
                typeof(ListViewSelectedItemsBehavior),
                new PropertyMetadata(null, OnSelectedItemsChanged));

        public static void SetSelectedItems(DependencyObject element, IList value)
        {
            element.SetValue(SelectedItemsProperty, value);
        }

        public static IList GetSelectedItems(DependencyObject element)
        {
            return (IList)element.GetValue(SelectedItemsProperty);
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView listView)
            {
                listView.SelectionChanged -= ListView_SelectionChanged;
                listView.SelectionChanged += ListView_SelectionChanged;
            }
        }

        private static void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                var boundSelectedItems = GetSelectedItems(listView);
                if (boundSelectedItems == null)
                {
                    return;
                }

                boundSelectedItems.Clear();

                foreach (var item in listView.SelectedItems)
                {
                    boundSelectedItems.Add(item);
                }
            }
        }
    }
}
