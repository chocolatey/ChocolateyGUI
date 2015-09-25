// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="VersionNumberProvider.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Providers
{
    using System.Linq;
    using System.Reflection;

    public class VersionNumberProvider : IVersionNumberProvider
    {
        private string _version;

        public virtual string Version
        {
            get
            {
                if (this._version != null)
                {
                    return this._version;
                }

                var assembly = this.GetType().Assembly;
                var informational =
                    ((AssemblyInformationalVersionAttribute[])assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute)))
                    .First();

                this._version = "Version: " + informational.InformationalVersion;
                return this._version;
            }
        }
    }
}