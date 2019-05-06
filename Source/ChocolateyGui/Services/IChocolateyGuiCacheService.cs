// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IChocolateyGuiCacheService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    public interface IChocolateyGuiCacheService
    {
        void PurgeIcons();

        void PurgeOutdatedPackages();
    }
}