// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IChocolateyConfigurationProvider.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Providers
{
    using System;
    using System.Collections.Generic;

    public interface IChocolateyConfigurationProvider
    {
        string ChocolateyInstall { get; }

        bool IsChocolateyExecutableBeingUsed { get; }

        IEnumerable<Tuple<string, string>> Sources { get; }
    }
}