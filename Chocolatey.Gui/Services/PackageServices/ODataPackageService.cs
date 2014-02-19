using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Threading.Tasks;
using Chocolatey.Gui.ChocolateyFeedService;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.Utilities.Extensions;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.Services.PackageServices
{
    public static class ODataPackageService
    {
        public static async Task<PackageSearchResults> Search(string query, Func<IPackageViewModel> packageFactory, Uri source)
        {
            return await Search(query, packageFactory, new PackageSearchOptions(), source);
        }

        public static async Task<PackageSearchResults> Search(string query, Func<IPackageViewModel> packageFactory, PackageSearchOptions options, Uri source)
        {
            var service = new FeedContext_x0060_1(source);
            var queryString = query;
            IQueryable<V2FeedPackage> feedQuery = service.Packages.Where(package => package.IsPrerelease == options.IncludePrerelease || package.IsPrerelease == false);

            if (!options.IncludeAllVersions)
                feedQuery = feedQuery.Where(package => package.IsLatestVersion || package.IsAbsoluteLatestVersion);

            if (!string.IsNullOrWhiteSpace(queryString))
            {
                feedQuery = options.MatchQuery ?
                    feedQuery.Where(package => package.Id == queryString || package.Title == queryString) :
                    feedQuery.Where(package => package.Id.Contains(queryString) || package.Title.Contains(queryString));
            }

            var totalCount = feedQuery.Count();

            if (!string.IsNullOrWhiteSpace(options.SortColumn))
            {

                feedQuery = !options.SortDescending ? feedQuery.OrderBy(options.SortColumn) : feedQuery.OrderByDescending(options.SortColumn);
            }

            feedQuery = feedQuery.Skip(options.CurrentPage * options.PageSize).Take(options.PageSize);
            var feedDataServiceQuery = (DataServiceQuery<V2FeedPackage>)feedQuery;
            var result = await Task.Factory.FromAsync(feedDataServiceQuery.BeginExecute, ar => feedDataServiceQuery.EndExecute(ar), null);

            var packages = result.Select(package => AutoMapper.Mapper.Map(package, packageFactory()));

            return new PackageSearchResults
            {
                TotalCount = totalCount,
                Packages = packages
            };
        }

        public static IPackageViewModel GetLatest(string id, Func<IPackageViewModel> packageFactory, Uri source, bool includePrerelease = false)
        {
            var service = new FeedContext_x0060_1(source);

            return service.Packages.Where(
                package =>
                    (package.IsPrerelease == includePrerelease || package.IsPrerelease == false) && package.Id == id)
                .Select(package => AutoMapper.Mapper.Map(package, packageFactory())).SingleOrDefault();
        }

        public static async Task<IPackageViewModel> EnsureIsLoaded(IPackageViewModel vm, Uri source)
        {
            var service = new FeedContext_x0060_1(source);

            var feedQuery =
                    (DataServiceQuery<V2FeedPackage>)service.Packages.Where(package => package.Id == vm.Id && package.Version == vm.Version.ToString());
            var result = await Task.Factory.FromAsync(feedQuery.BeginExecute, ar => feedQuery.EndExecute(ar), null);
            var v2FeedPackages = result as IList<V2FeedPackage> ?? result.ToList();
            if (result == null || !v2FeedPackages.Any())
                return vm;

            var packageInfo = v2FeedPackages.Single();
            return AutoMapper.Mapper.Map(packageInfo, vm);
        }
    }
}
