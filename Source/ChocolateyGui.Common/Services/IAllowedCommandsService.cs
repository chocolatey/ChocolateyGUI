// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IAllowedCommandsService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Services
{
    public interface IAllowedCommandsService
    {
        bool IsPinCommandAllowed { get; }

        bool IsInstallCommandAllowed { get; }

        bool IsUninstallCommandAllowed { get; }

        bool IsUpgradeCommandAllowed { get; }
    }
}