// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyConfigurationProvider.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Providers
{
    using System;
    using System.IO;
    using System.Linq;

    public class ChocolateyConfigurationProvider : IChocolateyConfigurationProvider
    {
        private string _chocolateyInstallLocation;
        private bool _isChocolateyExecutableBeingUsed;

        public ChocolateyConfigurationProvider()
        {
            this.GetChocolateyInstallLocation();
            this.DetermineIfChocolateyExecutableIsBeingUsed();
        }

        public string ChocolateyInstall
        {
            get
            {
                return this._chocolateyInstallLocation;
            }
        }

        public bool IsChocolateyExecutableBeingUsed
        {
            get
            {
                return this._isChocolateyExecutableBeingUsed;
            }
        }

        private void GetChocolateyInstallLocation()
        {
            this._chocolateyInstallLocation = Environment.GetEnvironmentVariable("ChocolateyInstall");
            if (string.IsNullOrWhiteSpace(this._chocolateyInstallLocation))
            {
                var pathVar = Environment.GetEnvironmentVariable("PATH");
                if (!string.IsNullOrWhiteSpace(pathVar))
                {
                    this._chocolateyInstallLocation =
                        pathVar.Split(';')
                            .SingleOrDefault(
                                path => path.IndexOf("Chocolatey", StringComparison.OrdinalIgnoreCase) > -1);
                }
            }

            if (string.IsNullOrWhiteSpace(this._chocolateyInstallLocation))
            {
                this._chocolateyInstallLocation = string.Empty;
            }
        }

        private void DetermineIfChocolateyExecutableIsBeingUsed()
        {
            var exePath = Path.Combine(this._chocolateyInstallLocation, "choco.exe");

            if (File.Exists(exePath))
            {
                this._isChocolateyExecutableBeingUsed = true;
            }
        }
    }
}