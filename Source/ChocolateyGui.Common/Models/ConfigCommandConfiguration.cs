// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ConfigCommandConfiguration.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ChocolateyGui.Common.Models
{
    [Serializable]
    public sealed class ConfigCommandConfiguration
    {
        public string Name { get; set; }

        public string ConfigValue { get; set; }

        public ConfigCommandType Command { get; set; }
    }
}