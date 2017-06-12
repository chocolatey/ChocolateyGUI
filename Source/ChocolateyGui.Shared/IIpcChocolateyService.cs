// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IIpcChocolateyService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ServiceModel;
using System.Threading.Tasks;
using ChocolateyGui.Models;

namespace ChocolateyGui
{
    [ServiceContract(CallbackContract = typeof(IIpcServiceCallbacks), SessionMode = SessionMode.Required)]
    public interface IIpcChocolateyService
    {
        [OperationContract]
        void Register();

        [OperationContract]
        Task<bool> IsElevated();

        [OperationContract]
        Task<Package[]> GetInstalledPackages();

        [OperationContract]
        Task<Tuple<string, string>[]> GetOutdatedPackages(bool includePrerelease = false);

        [OperationContract]
        Task<PackageResults> Search(string query, PackageSearchOptions options);

        [OperationContract]
        Task<Package> GetByVersionAndIdAsync(string id, string version, bool isPrerelease);

        [OperationContract]
        Task<PackageOperationResult> InstallPackage(
            string id,
            string version = null,
            Uri source = null,
            bool force = false);

        [OperationContract]
        Task<PackageOperationResult> UninstallPackage(string id, string version, bool force = false);

        [OperationContract]
        Task<PackageOperationResult> UpdatePackage(string id, Uri source = null);

        [OperationContract]
        Task<PackageOperationResult> PinPackage(string id, string version);

        [OperationContract]
        Task<PackageOperationResult> UnpinPackage(string id, string version);

        [OperationContract]
        Task<ChocolateyFeature[]> GetFeatures();

        [OperationContract]
        Task SetFeature(ChocolateyFeature feature);

        [OperationContract]
        Task<ChocolateySetting[]> GetSettings();

        [OperationContract]
        Task SetSetting(ChocolateySetting setting);

        [OperationContract]
        Task<ChocolateySource[]> GetSources();

        [OperationContract]
        Task AddSource(ChocolateySource source);

        [OperationContract]
        Task UpdateSource(string id, ChocolateySource source);

        [OperationContract]
        Task<bool> RemoveSource(string id);

        [OperationContract(IsOneWay = true)]
        void Unregister();

        [OperationContract(IsOneWay = true)]
        void Exit(bool restartingForAdmin = false);
    }
}