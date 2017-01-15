// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="RemoteSourceView.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.Views
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

            eventAggregator.Subscribe(this);
        }

        public void Handle(ResetScrollPositionMessage message)
        {
            if (Packages.Items.Count > 0)
            {
                Packages.ScrollIntoView(Packages.Items[0]);
            }
        }

        private void Packages_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var obj = (DependencyObject)e.OriginalSource;

            while (obj != null && obj != Packages)
            {
                if (obj.GetType() == typeof(ListBoxItem))
                {
                    var item = (ListBoxItem)obj;
                    var context = (IPackageViewModel)item.DataContext;
                    context.ViewDetails();
                }

                obj = VisualTreeHelper.GetParent(obj);
            }
        }
    }
}