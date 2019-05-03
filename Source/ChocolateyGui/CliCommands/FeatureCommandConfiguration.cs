// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="FeatureCommandConfiguration.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ChocolateyGui.CliCommands
{
    [Serializable]
    public sealed class FeatureCommandConfiguration
    {
        public string Name { get; set; }

        public FeatureCommandType Command { get; set; }
    }
}