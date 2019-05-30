// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IVersionNumberProvider.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Providers
{
    public interface IVersionNumberProvider
    {
        string Version { get; }
    }
}