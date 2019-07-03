// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="RemoteSourceView.xaml.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using ChocolateyGui.Common.Models.Messages;
using ChocolateyGui.Common.ViewModels.Items;

namespace ChocolateyGui.Common.Windows.Views
{
    /// <summary>
    ///     Interaction logic for RemoteSourceControl.xaml
    /// </summary>
    public partial class RemoteSourceView : IHandle<ResetScrollPositionMessage>
    {
        public RemoteSourceView(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }

            InitializeComponent();

            eventAggregator.Subscribe(this);

            this.Loaded += RemoteSourceViewOnLoaded;
        }

        public void Handle(ResetScrollPositionMessage message)
        {
            if (Packages.Items.Count > 0)
            {
                Packages.ScrollIntoView(Packages.Items[0]);
            }
        }

        private void RemoteSourceViewOnLoaded(object sender, RoutedEventArgs e)
        {
            this.SearchTextBox.Focus();
        }

        private void Packages_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var obj = (DependencyObject)e.OriginalSource;

            while (obj != null && obj != Packages)
            {
                var listBoxItem = obj as ListBoxItem;
                if (listBoxItem != null)
                {
                    var context = (IPackageViewModel)listBoxItem.DataContext;
                    context.ViewDetails();
                }

                obj = VisualTreeHelper.GetParent(obj);
            }
        }
    }
}