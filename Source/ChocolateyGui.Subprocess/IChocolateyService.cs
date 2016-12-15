// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IChocolateyService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChocolateyGui.Models;
using WampSharp.V2.Rpc;

namespace ChocolateyGui.Interface
{
    public interface IChocolateyService
    {
        [WampProcedure("com.chocolatey.iselevated")]
        Task<bool> IsElevated();

        [WampProcedure("com.chocolatey.getinstalled")]
        Task<IReadOnlyList<Package>> GetInstalledPackages();

        [WampProcedure("com.chocolatey.getoutdated")]
        Task<IReadOnlyList<Tuple<string, string>>> GetOutdatedPackages(bool includePrerelease = false);

        [WampProcedure("com.chocolatey.getbyversionandid")]
        Task<Package> GetByVersionAndIdAsync(string id, string version, bool isPrerelease);

        [WampProcedure("com.chocolatey.install")]
        Task<PackageOperationResult> InstallPackage(string id, string version = null, Uri source = null,
            bool force = false);

        [WampProcedure("com.chocolatey.uninstall")]
        Task<PackageOperationResult> UninstallPackage(string id, string version, bool force = false);

        [WampProcedure("com.chocolatey.update")]
        Task<PackageOperationResult> UpdatePackage(string id, Uri source = null);

        [WampProcedure("com.chocolatey.pin")]
        Task<PackageOperationResult> PinPackage(string id, string version);

        [WampProcedure("com.chocolatey.unpin")]
        Task<PackageOperationResult> UnpinPackage(string id, string version);

        [WampProcedure("com.chocolatey.kill")]
        void Exit();
    }
}
