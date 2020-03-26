// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ConfigCommandConfiguration.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
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