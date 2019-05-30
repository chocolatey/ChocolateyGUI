// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyGuiConfiguration.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ChocolateyGui.Common.Models
{
    [Serializable]
    public sealed class InformationCommandConfiguration
    {
        public string ChocolateyGuiVersion { get; set; }
        public string ChocolateyGuiProductVersion { get; set; }
        public string FullName { get; set; }
    }
}