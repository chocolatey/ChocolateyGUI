// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ODataPackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services.PackageServices
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Threading.Tasks;
    using ChocolateyGui.ChocolateyFeedService;
    using ChocolateyGui.Models;
    using ChocolateyGui.Utilities.Extensions;
    using ChocolateyGui.ViewModels.Items;

    public static class ODataPackageService
    {
        private static readonly object FeedLockObject = new object();

        private static readonly MemoryCache Cache = MemoryCache.Default;

        public static async Task<IPackageViewModel> EnsureIsLoaded(IPackageViewModel vm, Uri source)
        {
            var service = GetFeed(source);

            var feedQuery =
                    (DataServiceQuery<V2FeedPackage>)service.Packages.Where(package => package.Id == vm.Id && package.Version == vm.Version.ToString());
            var result = await Task.Factory.FromAsync(feedQuery.BeginExecute, ar => feedQuery.EndExecute(ar), null);
            var v2FeedPackages = result as IList<V2FeedPackage> ?? result.ToList();

            if (result == null || !v2FeedPackages.Any())
            {
                return null;
            }

            var packageInfo = v2FeedPackages.Single();
            return AutoMapper.Mapper.Map(packageInfo, vm);
        }

        public static async Task<IPackageViewModel> GetLatest(string id, Func<IPackageViewModel> packageFactory, Uri source, bool includePrerelease = false)
        {
            var service = GetFeed(source);

            var packageQuery = service.Packages.Where(p => p.IsPrerelease == includePrerelease || p.IsPrerelease == false)
                .Where(p => p.Id == id);

            packageQuery = includePrerelease ? packageQuery.Where(p => p.IsAbsoluteLatestVersion) : packageQuery.Where(p => p.IsLatestVersion);

            var feedDataServiceQuery = (DataServiceQuery<V2FeedPackage>)packageQuery;
            var packages = await Task.Factory.FromAsync(feedDataServiceQuery.BeginExecute, ar => feedDataServiceQuery.EndExecute(ar), null);
            var package = packages.FirstOrDefault();

            return package == null ? null : AutoMapper.Mapper.Map(package, packageFactory());
        }

        public static async Task<PackageSearchResults> Search(string query, Func<IPackageViewModel> packageFactory, Uri source)
        {
            return await Search(query, packageFactory, new PackageSearchOptions(), source);
        }

        public static async Task<PackageSearchResults> Search(string query, Func<IPackageViewModel> packageFactory, PackageSearchOptions options, Uri source)
        {
            var service = GetFeed(source);
            var queryString = query;
            IQueryable<V2FeedPackage> feedQuery = service.Packages.Where(package => package.IsPrerelease == options.IncludePrerelease || package.IsPrerelease == false);

            if (!options.IncludeAllVersions)
            {
                feedQuery = feedQuery.Where(package => package.IsLatestVersion || package.IsAbsoluteLatestVersion);
            }

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

        public static async Task<Exception> TestPath(Uri source)
        {
            try
            {
                var service = new FeedContext_x0060_1(source);
                var query = (DataServiceQuery<V2FeedPackage>)service.Packages.Take(1);
                var result = query.FirstOrDefault();
                await Task.Factory.FromAsync(query.BeginExecute, ar => query.EndExecute(ar), null);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        private static FeedContext_x0060_1 GetFeed(Uri source)
        {
            lock (FeedLockObject)
            {
                FeedContext_x0060_1 service;
                if ((service = (FeedContext_x0060_1)Cache.Get(GetMemoryCacheKey(source))) == null)
                {
                    service = new FeedContext_x0060_1(source)
                    {
                        IgnoreMissingProperties = true,
                        IgnoreResourceNotFoundException = true,
                        MergeOption = MergeOption.NoTracking
                    };

                    Cache.Set(
                        GetMemoryCacheKey(source),
                        service,
                        new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(20) });
                }

                return service;
            }
        }

        private static string GetMemoryCacheKey(Uri feed)
        {
            return string.Format(CultureInfo.CurrentCulture, "ODataPackageService.Feeds.{0}", feed);
        }
    }
}