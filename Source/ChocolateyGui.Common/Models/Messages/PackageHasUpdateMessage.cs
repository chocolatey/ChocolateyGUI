// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageHasUpdateMessage.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NuGet.Versioning;

namespace ChocolateyGui.Common.Models.Messages
{
    public class PackageHasUpdateMessage
    {
        public PackageHasUpdateMessage(string id, NuGetVersion version)
        {
            Id = id;
            Version = version;
        }

        public string Id { get; }

        public NuGetVersion Version { get; set; }
    }
}