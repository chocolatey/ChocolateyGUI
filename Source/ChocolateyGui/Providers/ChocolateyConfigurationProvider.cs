// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyConfigurationProvider.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public class ChocolateyConfigurationProvider : IChocolateyConfigurationProvider
    {
        private readonly Lazy<string> _chocolateyInstallLocation;
        private readonly Lazy<bool> _isChocolateyExecutableBeingUsed;

        public ChocolateyConfigurationProvider()
        {
            this._chocolateyInstallLocation = new Lazy<string>(this.GetChocolateyInstallLocation);
            this._isChocolateyExecutableBeingUsed = new Lazy<bool>(this.DetermineIfChocolateyExecutableIsBeingUsed);
        }

        public string ChocolateyInstall
        {
            get { return this._chocolateyInstallLocation.Value; }
        }

        public bool IsChocolateyExecutableBeingUsed
        {
            get { return this._isChocolateyExecutableBeingUsed.Value; }
        }

        public IEnumerable<Tuple<string, string>> Sources
        {
            get { return this.GetChocolateySources(); }
        }

        private string GetChocolateyInstallLocation()
        {
            var retVal = Environment.GetEnvironmentVariable("ChocolateyInstall");
            if (string.IsNullOrWhiteSpace(retVal))
            {
                var pathVar = Environment.GetEnvironmentVariable("PATH");
                if (!string.IsNullOrWhiteSpace(pathVar))
                {
                    retVal =
                        pathVar.Split(';')
                            .SingleOrDefault(
                                path => path.IndexOf("Chocolatey", StringComparison.OrdinalIgnoreCase) > -1);
                }
            }

            if (string.IsNullOrWhiteSpace(retVal))
            {
                retVal = string.Empty;
            }

            return retVal;
        }

        private bool DetermineIfChocolateyExecutableIsBeingUsed()
        {
            var exePath = Path.Combine(this._chocolateyInstallLocation.Value, "choco.exe");

            return File.Exists(exePath);
        }

        private IEnumerable<Tuple<string, string>> GetChocolateySources()
        {
            var chocoConfigFile = Path.Combine(this._chocolateyInstallLocation.Value, "config", "chocolatey.config");

            if (File.Exists(chocoConfigFile))
            {
                var configDoc = XDocument.Load(chocoConfigFile);

                return configDoc.Descendants("source")
                    .Select(e => Tuple.Create(e.Attribute("id").Value, e.Attribute("value").Value));
            }

            return Enumerable.Empty<Tuple<string, string>>();
        }
    }
}