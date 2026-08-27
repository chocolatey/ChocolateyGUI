// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="DebugInstallEnvironment.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if DEBUG
using System;
using System.IO;

namespace ChocolateyGui.Common.Startup
{
    public static class DebugInstallEnvironment
    {
        // When debugging Chocolatey GUI we do not want to read from / write to the
        // machine-wide Chocolatey installation (normally C:\ProgramData\chocolatey).
        // The bundled (official) chocolatey.lib resolves its install location from the
        // ChocolateyInstall environment variable, so point that at a folder beside the
        // GUI binaries. Debugging is then fully isolated from the installed Chocolatey
        // (its own lib, config, .chocolatey store, etc.) - mirroring how Chocolatey CLI
        // behaves when debugged out of Visual Studio. Set the environment variable
        // ChocolateyGuiUseSystemInstall=true to opt out and debug against the machine install,
        // or set ChocolateyGuiDebugInstall to a folder of your choosing (e.g. one outside the
        // bin directory so it survives a clean / rebuild).
        public static void RedirectChocolateyInstallForDebugging()
        {
            if (string.Equals(
                    Environment.GetEnvironmentVariable("ChocolateyGuiUseSystemInstall"),
                    "true",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var debugInstallLocation = Environment.GetEnvironmentVariable("ChocolateyGuiDebugInstall");
            if (string.IsNullOrWhiteSpace(debugInstallLocation))
            {
                debugInstallLocation = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            }

            Environment.SetEnvironmentVariable("ChocolateyInstall", debugInstallLocation);
        }
    }
}
#endif
