// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IBasePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ChocolateyGui.Models;
    using ChocolateyGui.ViewModels.Items;

    public interface IBasePackageService
    {
        event PackagesChangedEventHandler PackagesUpdated;

        void ClearPackageCache();

        Task<IEnumerable<IPackageViewModel>> GetPackagesFromLocalDirectory(Dictionary<string, string> requestedPackages, string directoryPath);

        bool IsPackageInstalled(string id, SemanticVersion version);
    }
}