// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SettingsView.xaml.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Caliburn.Micro;
using ChocolateyGui.Common.Models.Messages;

namespace ChocolateyGui.Common.Windows.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : IHandle<SourcesUpdatedMessage>
    {
        public SettingsView(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
            InitializeComponent();
        }

        public void Handle(SourcesUpdatedMessage message)
        {
            SourcesGrid.Items.Refresh();
        }
    }
}