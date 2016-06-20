// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalSourceView.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Input;
using Caliburn.Micro;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.Views
{
    /// <summary>
    ///     Interaction logic for LocalSourceView.xaml
    /// </summary>
    public partial class LocalSourceView
    {
        private readonly IEventAggregator _eventAggregator;

        public LocalSourceView(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
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