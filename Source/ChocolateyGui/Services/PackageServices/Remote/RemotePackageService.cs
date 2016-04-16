// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="RemotePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;
    using System.Threading.Tasks;
    using ChocolateyGui.Models;
    using ChocolateyGui.Services.PackageServices;
    using ChocolateyGui.ViewModels.Items;
    using NuGet;

    public class RemotePackageService : IRemotePackageService
    {
        private readonly IProgressService _progressService;
        private readonly Func<IPackageViewModel> _packageFactory;

        public RemotePackageService(
            IProgressService progressService,
            Func<IPackageViewModel> packageFactory)
        {
            _progressService = progressService;
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

            var results = await ODataRemotePackageService.Search(query, _packageFactory, options);
            await _progressService.StopLoading();
            return results;
        }

        public async Task<IPackageViewModel> GetLatest(string id, bool includePrerelease = false)
        {
            _progressService.WriteMessage(string.Format("Getting latest version of {0}...", id));
            return await ODataRemotePackageService.GetLatest(id, _packageFactory, includePrerelease);
        }

        public Task<IPackageViewModel> GetByVersionAndIdAsync(string id, SemanticVersion version, bool isPrerelease)
        {
            return ODataRemotePackageService.GetByVersionAndIdAsync(id, version, isPrerelease, _packageFactory);
        }
    }
}