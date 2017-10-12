// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyConfigurationProvider.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;

namespace ChocolateyGui.Providers
{
    public class ChocolateyConfigurationProvider : IChocolateyConfigurationProvider
    {
        public ChocolateyConfigurationProvider()
        {
            GetChocolateyInstallLocation();
            DetermineIfChocolateyExecutableIsBeingUsed();
        }

        public string ChocolateyInstall { get; private set; }

        public bool IsChocolateyExecutableBeingUsed { get; private set; }

        private void GetChocolateyInstallLocation()
        {
            ChocolateyInstall = Environment.GetEnvironmentVariable("ChocolateyInstall");
            if (string.IsNullOrWhiteSpace(ChocolateyInstall))
            {
                var pathVar = Environment.GetEnvironmentVariable("PATH");
                if (!string.IsNullOrWhiteSpace(pathVar))
                {
                    ChocolateyInstall =
                        pathVar.Split(';')
                            .SingleOrDefault(
                                path => path.IndexOf("Chocolatey", StringComparison.OrdinalIgnoreCase) > -1);
                }
            }

            if (string.IsNullOrWhiteSpace(ChocolateyInstall))
            {
                ChocolateyInstall = string.Empty;
            }
        }

        private void DetermineIfChocolateyExecutableIsBeingUsed()
        {
            var exePath = Path.Combine(ChocolateyInstall, "choco.exe");

            if (File.Exists(exePath))
            {
                IsChocolateyExecutableBeingUsed = true;
            }
        }
    }
}