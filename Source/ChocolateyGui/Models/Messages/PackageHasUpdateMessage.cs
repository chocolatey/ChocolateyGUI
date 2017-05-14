// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageHasUpdateMessage.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models.Messages
{
    public class PackageHasUpdateMessage
    {
        public PackageHasUpdateMessage(string id, SemanticVersion version)
        {
            Id = id;
            Version = version;
        }

        public string Id { get; }

        public SemanticVersion Version { get; set; }
    }
}