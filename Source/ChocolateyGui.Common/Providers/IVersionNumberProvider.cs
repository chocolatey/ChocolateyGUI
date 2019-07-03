// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IVersionNumberProvider.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Providers
{
    public interface IVersionNumberProvider
    {
        string Version { get; }
    }
}