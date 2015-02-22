// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="VersionNumberProvider.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Providers
{
    using System;

    public class VersionNumberProvider : IVersionNumberProvider
    {
        public virtual string Version
        {
            get
            {
                var assembly = this.GetType().Assembly;
                var fullName = assembly.FullName;
                var i = fullName.IndexOf("Version=", StringComparison.Ordinal);
                return i < 0 ? fullName : fullName.Substring(i).Split(' ', ',')[0];
            }
        }
    }
}