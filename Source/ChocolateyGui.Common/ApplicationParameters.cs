// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ApplicationParameters.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common
{
    using chocolatey.infrastructure.filesystem;

    public static class ApplicationParameters
    {
        private static readonly IFileSystem _fileSystem = new DotNetFileSystem();

#if DEBUG
#pragma warning disable SA1202 // ElementsMustBeOrderedByAccess
        public static readonly string InstallLocation = _fileSystem.get_directory_name(_fileSystem.get_current_assembly_path());
#pragma warning restore SA1202 // ElementsMustBeOrderedByAccess
        public static readonly string LicensedGuiAssemblyLocation = _fileSystem.file_exists(_fileSystem.combine_paths(InstallLocation, "chocolateygui.licensed.dll")) ? _fileSystem.combine_paths(InstallLocation, "chocolateygui.licensed.dll") : _fileSystem.combine_paths(InstallLocation, "extensions", "chocolateygui", "chocolateygui.licensed.dll");
#else
#pragma warning disable SA1202 // ElementsMustBeOrderedByAccess
        public static readonly string ChocolateyInstallEnvironmentVariableName = "ChocolateyInstall";
#pragma warning restore SA1202 // ElementsMustBeOrderedByAccess
        public static readonly string InstallLocation = System.Environment.GetEnvironmentVariable(ChocolateyInstallEnvironmentVariableName) ?? _fileSystem.get_directory_name(_fileSystem.get_current_assembly_path());
        public static readonly string LicensedGuiAssemblyLocation = _fileSystem.combine_paths(InstallLocation, "extensions", "chocolateygui", "chocolateygui.licensed.dll");
#endif

        public static readonly string Name = "Chocolatey GUI";

        public static readonly string FeatureCommandName = "Feature";

        public static readonly string ConfigCommandName = "Config";

        public static readonly string PurgeCommandName = "Purge";

        public static readonly string LicensedChocolateyGuiAssemblySimpleName = "chocolateygui.licensed";
    }
}