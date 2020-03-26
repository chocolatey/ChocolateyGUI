// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="VersionNumberProvider.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Reflection;
using ChocolateyGui.Common.Properties;

namespace ChocolateyGui.Common.Providers
{
    public class VersionNumberProvider : IVersionNumberProvider
    {
        private string _version;

        public virtual string Version
        {
            get
            {
                if (_version != null)
                {
                    return _version;
                }

                var assembly = GetType().Assembly;
                var informational =
                    ((AssemblyInformationalVersionAttribute[])assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute)))
                        .First();

                _version = string.Format(Resources.VersionNumberProvider_VersionFormat, informational.InformationalVersion);
                return _version;
            }
        }
    }
}