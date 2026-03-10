// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IChocolateyGuiCacheService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Services
{
    using System;
    using ChocolateyGui.Common.Models;

    public interface IChocolateyGuiCacheService
    {
        void PurgeIcons();

        [Obsolete("Use the new overload that includes passing in the source and whether or not to include prerelease package versions.")]
        void PurgeOutdatedPackages();

        void PurgeOutdatedPackages(ChocolateySource source, bool includePrerelease);
    }
}