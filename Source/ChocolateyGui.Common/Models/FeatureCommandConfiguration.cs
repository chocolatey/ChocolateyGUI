// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="FeatureCommandConfiguration.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ChocolateyGui.Common.Models
{
    [Serializable]
    public sealed class FeatureCommandConfiguration
    {
        public string Name { get; set; }

        public FeatureCommandType Command { get; set; }
    }
}