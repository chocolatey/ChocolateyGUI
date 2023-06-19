// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AdvancedInstall.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Models
{
    public class AdvancedInstall
    {
        public string PackageParameters { get; set; }

        public string InstallArguments { get; set; }

        public int ExecutionTimeoutInSeconds { get; set; }

        public string CacheLocation { get; set; }

        public string LogFile { get; set; }

        public bool PreRelease { get; set; }

        public bool Forcex86 { get; set; }

        public bool OverrideArguments { get; set; }

        public bool NotSilent { get; set; }

        public bool ApplyInstallArgumentsToDependencies { get; set; }

        public bool ApplyPackageParametersToDependencies { get; set; }

        public bool AllowDowngrade { get; set; }

        public bool IgnoreDependencies { get; set; }

        public bool ForceDependencies { get; set; }

        public bool SkipPowerShell { get; set; }

        public bool IgnoreHttpCache { get; set; }

        public bool IgnoreChecksums { get; set; }

        public bool AllowEmptyChecksums { get; set; }

        public bool AllowEmptyChecksumsSecure { get; set; }

        public bool RequireChecksums { get; set; }

        public string DownloadChecksum { get; set; }

        public string DownloadChecksum64bit { get; set; }

        public string DownloadChecksumType { get; set; }

        public string DownloadChecksumType64bit { get; set; }
    }
}