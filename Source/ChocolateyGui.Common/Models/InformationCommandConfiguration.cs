// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="InformationCommandConfiguration.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
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

        public string DisplayVersion { get; set; }

        public string FullName { get; set; }
    }
}