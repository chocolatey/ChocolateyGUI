// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyFeature.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Models
{
    public class ChocolateyFeature
    {
        public string Name { get; set; }

        public bool Enabled { get; set; }

        public bool SetExplicitly { get; set; }

        public string Description { get; set; }
    }
}