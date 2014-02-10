using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Chocolatey.Gui.ChocolateyFeedService;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.ViewModels.Items;
using Chocolatey.Gui.Utilities.Extensions;

namespace Chocolatey.Gui.Services
{
    public class PackageService : IPackageService
    {
        private FeedContext_x0060_1 _service;
        public async Task<PackageSearchResults> Search(string query)
        {
            return await Search(query, new PackageSearchOptions());
        }

        public async Task<PackageSearchResults> Search(string query, PackageSearchOptions options)
        {
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

            var packages = result.Select(package => AutoMapper.Mapper.Map(package, App.Container.Resolve<IPackageViewModel>()));
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
                .Select(package => AutoMapper.Mapper.Map(package, App.Container.Resolve<IPackageViewModel>())).SingleOrDefault();
        }
    }
}
