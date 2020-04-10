// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AllowedCommandsService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Services
{
    public class AllowedCommandsService : IAllowedCommandsService
    {
        public bool IsPinCommandAllowed
        {
            get { return true; }
        }

        public bool IsInstallCommandAllowed
        {
            get { return true; }
        }

        public bool IsUninstallCommandAllowed
        {
            get { return true; }
        }

        public bool IsUpgradeCommandAllowed
        {
            get { return true; }
        }
    }
}