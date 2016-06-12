// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourcesViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Caliburn.Micro;
using ChocolateyGui.Services;
////using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.ViewModels
{
    public sealed class SourcesViewModel : Conductor<ISourceViewModelBase>.Collection.OneActive
    {
        public SourcesViewModel(ISourceService sourceService,
            Func<string, LocalSourceViewModel> localSourceVmFactory,
            Func<Uri, string, RemoteSourceViewModel> remoteSourceVmFactory)
        {
            if (sourceService == null)
            {
                throw new ArgumentNullException(nameof(sourceService));
            }

            Items.Add(localSourceVmFactory("This PC"));

            foreach (var source in sourceService.GetSources())
            {
                Items.Add(remoteSourceVmFactory(new Uri(source.Url), source.Name));
            }

            // ActiveItem = Items[0];
        }
    }
}