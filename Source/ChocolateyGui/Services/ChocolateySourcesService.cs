// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChocolateySourcesService.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using chocolatey;
using chocolatey.infrastructure.app.configuration;
using chocolatey.infrastructure.app.domain;
using ChocolateyGui.Models;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.Services
{
    public class ChocolateySourcesService : ISourceService
    {
        private const string SourceCommnadName = "source";

#pragma warning disable CS0067 // The event 'ChocolateySourcesService.SourcesChanged' is never used
        public event SourcesChangedEventHandler SourcesChanged;
#pragma warning restore CS0067 // The event 'ChocolateySourcesService.SourcesChanged' is never used

        public void AddSource(SourceViewModel sourceViewModel)
        {
            var choco = Lets.GetChocolatey();
            choco.Set(
                config =>
                {
                    config.CommandName = SourceCommnadName;
                    config.SourceCommand.Command = SourceCommandType.add;
                    config.SourceCommand.Name = sourceViewModel.Name;
                    config.Sources = sourceViewModel.Url.to_string();
                    config.AllowUnofficialBuild = true;
                });

            choco.Run();
        }

        public SourceViewModel GetDefaultSource()
        {
            return
                GetSourcesImpl()
                    .Select(source => new SourceViewModel {Name = source.Id, Url = source.Value})
                    .FirstOrDefault();
        }

        public IEnumerable<SourceViewModel> GetSources()
        {
            return GetSourcesImpl().Select(source => new SourceViewModel {Name = source.Id, Url = source.Value});
        }

        public void RemoveSource(SourceViewModel sourceViewModel)
        {
            var choco = Lets.GetChocolatey();
            choco.Set(
                config =>
                {
                    config.CommandName = SourceCommnadName;
                    config.SourceCommand.Command = SourceCommandType.remove;
                    config.SourceCommand.Name = sourceViewModel.Name;
                    config.AllowUnofficialBuild = true;
                });

            choco.Run();
        }

        private static IEnumerable<ChocolateySource> GetSourcesImpl()
        {
            var choco = Lets.GetChocolatey();
            choco.Set(
                config =>
                {
                    config.CommandName = SourceCommnadName;
                    config.SourceCommand.Command = SourceCommandType.list;
                    config.AllowUnofficialBuild = true;
                });

            return choco.List<ChocolateySource>().OrderByDescending(source => source.Priority);
        }
    }
}