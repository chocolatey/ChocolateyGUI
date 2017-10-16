// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SettingsView.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Windows.Input;
using Caliburn.Micro;
using ChocolateyGui.Models.Messages;

namespace ChocolateyGui.Views
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

        private void HandleMarkdownLink(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Parameter.ToString()));
        }
    }
}