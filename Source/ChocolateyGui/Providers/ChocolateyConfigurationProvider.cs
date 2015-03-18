// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyConfigurationProvider.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Providers
{
    using System;
    using System.Linq;
    using ChocolateyGui.Services;

    public class ChocolateyConfigurationProvider : IChocolateyConfigurationProvider
    {
        private readonly ILogService _logService;

        public ChocolateyConfigurationProvider(Func<Type, ILogService> logServiceFunc)
        {
            this._logService = logServiceFunc(typeof(ChocolateyService));
        }

        public string ChocolateyInstall
        {
            get
            {
                return this.GetChocolateyInstallLocation();
            }
        }

        private string GetChocolateyInstallLocation()
        {
            var chocolateyInstallLocation = Environment.GetEnvironmentVariable("ChocolateyInstall");
            if (string.IsNullOrWhiteSpace(chocolateyInstallLocation))
            {
                var pathVar = Environment.GetEnvironmentVariable("PATH");
                if (!string.IsNullOrWhiteSpace(pathVar))
                {
                    chocolateyInstallLocation =
                        pathVar.Split(';')
                            .SingleOrDefault(
                                path => path.IndexOf("Chocolatey", StringComparison.OrdinalIgnoreCase) > -1);
                }
            }

            if (!string.IsNullOrWhiteSpace(chocolateyInstallLocation))
            {
                return chocolateyInstallLocation;
            }

            this._logService.Warn("Unable to find chocolatey install directory!");
            return string.Empty;
        }
    }
}