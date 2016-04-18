// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="FileSystemPackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services.PackageServices
{
    using System;
    using System.Threading.Tasks;
    using ChocolateyGui.ViewModels.Items;

    public static class FileSystemPackageService
    {
        public static Task<IPackageViewModel> EnsureIsLoaded(IPackageViewModel viewModel)
        {
            return Task.Run(() => viewModel);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "chocolateyService", Justification = "Will be reviewed after being implemented")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "includePrerelease", Justification = "Will be reviewed after being implemented")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "id", Justification = "Will be reviewed after being implemented")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "packageFactory", Justification = "Will be reviewed after being implemented")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "source", Justification = "Will be reviewed after being implemented")]
        public static IPackageViewModel GetLatest(string id, IChocolateyPackageService chocolateyService, Func<IPackageViewModel> packageFactory, Uri source, bool includePrerelease = false)
        {
            throw new NotImplementedException();
        }
    }
}