// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IChocolateyConfigurationProvider.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Providers
{
    public interface IChocolateyConfigurationProvider
    {
        string ChocolateyInstall { get; }

        bool IsChocolateyExecutableBeingUsed { get; }
    }
}