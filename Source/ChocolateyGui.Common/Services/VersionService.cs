// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="VersionService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System.Reflection;

namespace ChocolateyGui.Common.Services
{
    public class VersionService : IVersionService
    {
        public string InformationalVersion
        {
            get { return Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion; }
        }

        public string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public string DisplayVersion
        {
            get { return string.Format("{0} v{1}", "Chocolatey GUI", Version); }
        }
    }
}