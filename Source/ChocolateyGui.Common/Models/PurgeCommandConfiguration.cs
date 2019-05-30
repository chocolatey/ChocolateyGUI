// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PurgeCommandConfiguration.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ChocolateyGui.Common.Models
{
    [Serializable]
    public sealed class PurgeCommandConfiguration
    {
        public PurgeCommandType Command { get; set; }
    }
}