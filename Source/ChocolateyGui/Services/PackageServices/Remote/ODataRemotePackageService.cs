// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ODataRemotePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services.PackageServices
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using chocolatey;
    using chocolatey.infrastructure.app.domain;
    using chocolatey.infrastructure.results;
    using ChocolateyGui.Models;
    using ChocolateyGui.Utilities.Extensions;
    using ChocolateyGui.ViewModels.Items;
    using NuGet;

    public static class ODataRemotePackageService
    {
        public static async Task<IPackageViewModel> GetLatest(string id, Func<IPackageViewModel> packageFactory, bool includePrerelease = false)
        {
            var choco = Lets.GetChocolatey();
            choco.Set(config =>
            {
                config.CommandName = CommandNameType.list.ToString();
                config.Input = id;
                config.AllowUnofficialBuild = true;
                config.ListCommand.ByIdOnly = true;
                config.Prerelease = includePrerelease;
                config.ListCommand.Exact = true;
            });

            var packageResults = await choco.ListAsync<PackageResult>();
            var package = packageResults
                .Where(result => string.Equals(result.Package.Id, id, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault(
                    result =>
                        includePrerelease ? result.Package.IsAbsoluteLatestVersion : result.Package.IsLatestVersion);
            
            var mappedPackage = package == null ? null : AutoMapper.Mapper.Map(package.Package, packageFactory());
            if (mappedPackage != null)
            {
                mappedPackage.IsInstalled = !string.IsNullOrWhiteSpace(package.InstallLocation);
            }

            return mappedPackage;
        }

        public static async Task<PackageSearchResults> Search(string query, Func<IPackageViewModel> packageFactory)
        {
            return await Search(query, packageFactory, new PackageSearchOptions());
        }

        public static async Task<PackageSearchResults> Search(string query, Func<IPackageViewModel> packageFactory, PackageSearchOptions options)
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
                config.ListCommand.OrderByPopularity = string.IsNullOrWhiteSpace(options.SortColumn) ||
                                                        options.SortColumn == "DownloadCount";
                config.ListCommand.Exact = options.MatchQuery;
            });

            var packages = (await choco.ListAsync<PackageResult>()).Select(package =>
            {
                var mappedPackage = AutoMapper.Mapper.Map(package.Package, packageFactory());
                mappedPackage.IsInstalled = !string.IsNullOrWhiteSpace(package.InstallLocation);
                return mappedPackage;
            });

            return new PackageSearchResults
            {
                Packages = packages,
                TotalCount = await Task.Run(() => choco.ListCount())
            };
        }

        public static async Task<IPackageViewModel> GetByVersionAndIdAsync(string id, SemanticVersion version, bool isPrerelease, Func<IPackageViewModel> packageFactory)
        {
            var choco = Lets.GetChocolatey();
            choco.Set(config =>
            {
                config.CommandName = CommandNameType.list.ToString();
                config.Input = id;
                config.AllowUnofficialBuild = true;
                config.ListCommand.ByIdOnly = true;
                config.Prerelease = isPrerelease;
                config.ListCommand.Exact = true;
                config.AllVersions = true;
            });

            var packageResults = await choco.ListAsync<PackageResult>();
            var package = packageResults
                .Where(result => string.Equals(result.Package.Id, id, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault(
                    result => result.Package.Version == version);

            var mappedPackage = package == null ? null : AutoMapper.Mapper.Map(package.Package, packageFactory());
            if (mappedPackage != null)
            {
                mappedPackage.IsInstalled = !string.IsNullOrWhiteSpace(package.InstallLocation);
            }

            return mappedPackage;
        }
    }
}