// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="RemotePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using chocolatey;
using chocolatey.infrastructure.app.domain;
using chocolatey.infrastructure.results;
using ChocolateyGui.Models;
using ChocolateyGui.Utilities.Extensions;
using ChocolateyGui.ViewModels.Items;
using NuGet;

namespace ChocolateyGui.Services
{
    public class RemotePackageService : IRemotePackageService
    {
        private readonly Func<string, ILogService> _logFactory;
        private readonly IMapper _mapper;
        private readonly Func<IPackageViewModel> _packageFactory;
        private readonly IProgressService _progressService;

        public RemotePackageService(
            IProgressService progressService,
            IMapper mapper,
            Func<string, ILogService> logFactory,
            Func<IPackageViewModel> packageFactory)
        {
            _progressService = progressService;
            _mapper = mapper;
            _logFactory = logFactory;
            _packageFactory = packageFactory;
        }

        public async Task<PackageSearchResults> Search(string query)
        {
            return await Search(query, new PackageSearchOptions());
        }

        public async Task<PackageSearchResults> Search(string query, PackageSearchOptions options)
        {
            await _progressService.StartLoading("Search");
            if (string.IsNullOrWhiteSpace(query))
            {
                _progressService.WriteMessage("Loading data...");
            }
            else
            {
                _progressService.WriteMessage(string.Format("Searching for {0}", query));
            }

            var results = await SearchImpl(query, options);
            await _progressService.StopLoading();
            return results;
        }

        public async Task<IPackageViewModel> GetLatest(string id, bool includePrerelease = false)
        {
            _progressService.WriteMessage(string.Format("Getting latest version of {0}...", id));
            var choco = Lets.GetChocolatey().Init(_progressService, _logFactory);
            choco.Set(config =>
            {
                config.CommandName = CommandNameType.list.ToString();
                config.Input = id;
                config.AllowUnofficialBuild = true;
                config.ListCommand.ByIdOnly = true;
                config.Prerelease = includePrerelease;
                config.ListCommand.Exact = true;
#if !DEBUG
                config.Verbose = false;
#endif // DEBUG
            });

            var packageResults = await choco.ListAsync<PackageResult>();
            var package = packageResults
                .FirstOrDefault(
                    result =>
                        includePrerelease ? result.Package.IsAbsoluteLatestVersion : result.Package.IsLatestVersion);

            return GetMappedPackage(package, _packageFactory);
        }

        public async Task<IPackageViewModel> GetByVersionAndIdAsync(string id, SemanticVersion version,
            bool isPrerelease)
        {
            var choco = Lets.GetChocolatey().Init(_progressService, _logFactory);
            choco.Set(config =>
            {
                config.CommandName = CommandNameType.list.ToString();
                config.Input = id;
                config.AllowUnofficialBuild = true;
                config.ListCommand.ByIdOnly = true;
                config.Prerelease = isPrerelease;
                config.ListCommand.Exact = true;
                config.AllVersions = true;
#if !DEBUG
                config.Verbose = false;
#endif // DEBUG
            });

            var packageResults = await choco.ListAsync<PackageResult>();
            var package = packageResults
                .Where(result => string.Equals(result.Package.Id, id, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault(
                    result => result.Package.Version == version);

            return GetMappedPackage(package, _packageFactory);
        }

        private IPackageViewModel GetMappedPackage(PackageResult package, Func<IPackageViewModel> packageFactory)
        {
            var mappedPackage = package == null ? null : _mapper.Map(package.Package, packageFactory());
            return mappedPackage;
        }

        private async Task<PackageSearchResults> SearchImpl(string query, PackageSearchOptions options)
        {
            var choco = Lets.GetChocolatey().Init(_progressService, _logFactory);
            choco.Set(config =>
            {
                config.CommandName = CommandNameType.list.ToString();
                config.Input = query;
                config.AllowUnofficialBuild = true;
                config.AllVersions = options.IncludeAllVersions;
                config.Prerelease = options.IncludePrerelease;
                config.ListCommand.Page = options.CurrentPage;
                config.ListCommand.PageSize = options.PageSize;
                config.ListCommand.OrderByPopularity = string.IsNullOrWhiteSpace(options.SortColumn) ||
                                                       options.SortColumn == "DownloadCount";
                config.ListCommand.Exact = options.MatchQuery;
#if !DEBUG
                config.Verbose = false;
#endif // DEBUG
            });

            var packages = (await choco.ListAsync<PackageResult>()).Select(package =>
            {
                var mappedPackage = _mapper.Map(package.Package, _packageFactory());
                mappedPackage.IsInstalled = !string.IsNullOrWhiteSpace(package.InstallLocation);
                return mappedPackage;
            });

            return new PackageSearchResults
            {
                Packages = packages,
                TotalCount = await Task.Run(() => choco.ListCount())
            };
        }
    }
}