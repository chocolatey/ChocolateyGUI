// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="FileSystemPackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services.PackageServices
{
    using ChocolateyGui.Models;
    using ChocolateyGui.Utilities.Extensions;
    using ChocolateyGui.ViewModels.Items;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Threading.Tasks;

    public static class FileSystemPackageService
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;

        private static string GetMemoryCacheKey(Uri source, string query, PackageSearchOptions options)
        {
            return string.Format("FileSystemPackageService.QueryResult.{0}|{1}|{2}|{3}", source, query, options.IncludeAllVersions, options.IncludePrerelease);
        }

        public static async Task<PackageSearchResults> Search(string queryString, IChocolateyService chocolateyService, Func<IPackageViewModel> packageFactory, Uri source)
        {
            return await Search(queryString, chocolateyService, packageFactory, new PackageSearchOptions(), source);
        }

        public static async Task<PackageSearchResults> Search(string queryString, IChocolateyService chocolateyService, Func<IPackageViewModel> packageFactory, PackageSearchOptions options, Uri source)
        {
            List<IPackageViewModel> packages;
            if ((packages = (List<IPackageViewModel>)Cache.Get(GetMemoryCacheKey(source, queryString, options))) == null)
            {
                var queryCommand = string.Format("list {0} {1} {2} -source \"{3}\"", queryString,
                    options.IncludePrerelease ? "-pre" : "", options.IncludeAllVersions ? "-all" : string.Empty, source);

                var chocoPackageList = (await chocolateyService.RunIndirectChocolateyCommand(queryCommand, false))
                    .ToDictionary(o => o.ToString().Split(' ')[0], o => o.ToString().Split(' ')[1]);

                packages = (await chocolateyService.GetPackagesFromLocalDirectory(chocoPackageList, source.ToString())).ToList();

                Cache.Set(GetMemoryCacheKey(source, queryString, options), packages, new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1)
                });
            }

            IQueryable<IPackageViewModel> query = packages.AsQueryable();
            if (!string.IsNullOrWhiteSpace(options.SortColumn))
            {
                query = options.SortDescending
                    ? query.OrderByDescending(options.SortColumn)
                    : query.OrderBy(options.SortColumn);
            }

            return new PackageSearchResults
            {
                Packages = query.Skip(options.CurrentPage * options.PageSize).Take(options.PageSize),
                TotalCount = packages.Count
            };
        }

        public static IPackageViewModel GetLatest(string id, IChocolateyService chocolateyService, Func<IPackageViewModel> packageFactory, Uri source, bool includePrerelease = false)
        {
            throw new NotImplementedException();
        }

        public static Task<IPackageViewModel> EnsureIsLoaded(IPackageViewModel vm, Uri source)
        {
            return TaskEx.Run(() => vm);
        }

        public static async Task<bool> TestPath(Uri source, IChocolateyService chocolateyService)
        {
            return
                (await
                    chocolateyService.RunIndirectChocolateyCommand(string.Format("list -source \"{0}\"", source), false)).Count > 0;
        }
    }
}