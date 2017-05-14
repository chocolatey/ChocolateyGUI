// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyFeature.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    public class ChocolateyFeature
    {
        public string Name { get; set; }

        public bool Enabled { get; set; }

        public bool SetExplicitly { get; set; }

        public string Description { get; set; }
    }
}