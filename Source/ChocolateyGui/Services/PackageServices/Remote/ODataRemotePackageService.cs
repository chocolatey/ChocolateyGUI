// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ODataRemotePackageService.cs">
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
    using chocolatey;
    using chocolatey.infrastructure.app.domain;
    using chocolatey.infrastructure.results;
    using ChocolateyGui.ChocolateyFeedService;
    using ChocolateyGui.Models;
    using ChocolateyGui.Utilities.Extensions;
    using ChocolateyGui.ViewModels.Items;
    using NuGet;

    public static class ODataRemotePackageService
    {
        private static readonly object FeedLockObject = new object();

        private static readonly MemoryCache Cache = MemoryCache.Default;

        static ODataRemotePackageService()
        {
            AutoMapper.Mapper.CreateMap<IPackage, IPackageViewModel>();
        }

        public static async Task<IPackageViewModel> EnsureIsLoaded(IPackageViewModel viewModel, Uri source)
        {
            var service = GetFeed(source);

            var feedQuery =
                    (DataServiceQuery<V2FeedPackage>)service.Packages.Where(package => package.Id == viewModel.Id && package.Version == viewModel.Version.ToString());
            var result = await Task.Factory.FromAsync(feedQuery.BeginExecute, ar => feedQuery.EndExecute(ar), null);
            var v2FeedPackages = result as IList<V2FeedPackage> ?? result.ToList();

            if (result == null || !v2FeedPackages.Any())
            {
                return null;
            }

            var packageInfo = v2FeedPackages.Single();
            return AutoMapper.Mapper.Map(packageInfo, viewModel);
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
            var choco = Lets.GetChocolatey();
            choco.Set(config =>
            {
                config.CommandName = CommandNameType.list.ToString();
                config.Input = query;
                config.AllowUnofficialBuild = true;
                config.AllVersions = options.IncludeAllVersions;
                config.Prerelease = options.IncludePrerelease;
                config.ListCommand.Page = options.CurrentPage;
                config.ListCommand.PageSize = options.PageSize;
            });

            var packages = (await choco.ListAsync<PackageResult>()).Select(package => AutoMapper.Mapper.Map(package.Package, packageFactory()));

            return new PackageSearchResults
            {
                Packages = packages,
                TotalCount = await Task.Run(() => choco.ListCount())
            };
        }

        public static async Task<Exception> TestPath(Uri source)
        {
            try
            {
                var service = GetFeed(source);
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
            return string.Format(CultureInfo.CurrentCulture, "ODataRemotePackageService.Feeds.{0}", feed);
        }
    }
}