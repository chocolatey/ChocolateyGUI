using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Threading.Tasks;
using Chocolatey.Gui.ChocolateyFeedService;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.Properties;
using Chocolatey.Gui.ViewModels.Items;
using Chocolatey.Gui.Utilities.Extensions;

namespace Chocolatey.Gui.Services
{
    public class PackageService : IPackageService
    {
        private readonly IProgressService _progressService;
        private readonly Func<IPackageViewModel> _packageFactory;
        public PackageService(IProgressService progressService, Func<IPackageViewModel> packageFactory)
        {
            _progressService = progressService;
            _packageFactory = packageFactory;
        }

        private FeedContext_x0060_1 _service;
        public async Task<PackageSearchResults> Search(string query)
        {
            return await Search(query, new PackageSearchOptions());
        }

        public async Task<PackageSearchResults> Search(string query, PackageSearchOptions options)
        {
            _progressService.StartLoading();

            var queryString = query;
            var feedQuery = (DataServiceQuery<V2FeedPackage>)_service.Packages.Where(package => package.IsPrerelease == options.IncludePrerelease || package.IsPrerelease == false);

            if (!options.IncludeAllVersions)
                feedQuery = (DataServiceQuery<V2FeedPackage>)feedQuery.Where(package => package.IsLatestVersion || package.IsAbsoluteLatestVersion);

            if (!string.IsNullOrWhiteSpace(queryString))
            {
                feedQuery = options.MatchQuery ?
                    (DataServiceQuery<V2FeedPackage>)feedQuery.Where(package => package.Id == queryString || package.Title == queryString) :
                    (DataServiceQuery<V2FeedPackage>)feedQuery.Where(package => package.Id.Contains(queryString) || package.Title.Contains(queryString));
            }

            var totalCount = feedQuery.Count();

            if (!string.IsNullOrWhiteSpace(options.SortColumn))
            {

                feedQuery = !options.SortDescending ? (DataServiceQuery<V2FeedPackage>)feedQuery.OrderBy(options.SortColumn) : (DataServiceQuery<V2FeedPackage>)feedQuery.OrderByDescending(options.SortColumn);
            }

            feedQuery = (DataServiceQuery<V2FeedPackage>)feedQuery.Skip(options.CurrentPage * options.PageSize).Take(options.PageSize);
            var result = await Task.Factory.FromAsync((cb, o) => feedQuery.BeginExecute(cb, o), ar => feedQuery.EndExecute(ar), null);

            var packages = result.Select(package => AutoMapper.Mapper.Map(package ,_packageFactory()));

            _progressService.StopLoading();

            return new PackageSearchResults
            {
                TotalCount = totalCount,
                Packages = packages
            };
        }

        public void SetSource(string packageUrl)
        {
           _service = new FeedContext_x0060_1(new Uri(packageUrl));
        }

        public void SetSource(Uri packageUri)
        {
            _service = new FeedContext_x0060_1(packageUri);
        }

        public IPackageViewModel GetLatest(string id, bool includePrerelease = false)
        {
            if (_service == null)
                return null;

            return _service.Packages.Where(
                package =>
                    (package.IsPrerelease == includePrerelease || package.IsPrerelease == false) && package.Id == id)
                .Select(package => AutoMapper.Mapper.Map(package, _packageFactory())).SingleOrDefault();
        }

        public async Task<IPackageViewModel> EnsureIsLoaded(IPackageViewModel vm)
        {
            _progressService.StartLoading();
            if (_service == null)
            {
                var currentSource = Settings.Default.currentSource;
                var sources = Settings.Default.sources;
                foreach (var parts in (from string source in sources select source.Split('|')).Where(parts => parts[0] == currentSource))
                {
                    SetSource(parts[1]);
                }
            }

            var feedQuery =
                    (DataServiceQuery<V2FeedPackage>)_service.Packages.Where(package => package.Id == vm.Id && package.Version == vm.Version.ToString());
            var result = await Task.Factory.FromAsync(feedQuery.BeginExecute, ar => feedQuery.EndExecute(ar), null);
            var v2FeedPackages = result as IList<V2FeedPackage> ?? result.ToList();
            if (result == null || !v2FeedPackages.Any())
                return vm;

            var packageInfo = v2FeedPackages.Single();
            _progressService.StopLoading();
            return AutoMapper.Mapper.Map(packageInfo, vm);
        }
    }
}
