// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IVersionService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Services
{
    public interface IVersionService
    {
        string InformationalVersion { get; }

        string Version { get;  }

        string DisplayVersion { get; }
    }
}