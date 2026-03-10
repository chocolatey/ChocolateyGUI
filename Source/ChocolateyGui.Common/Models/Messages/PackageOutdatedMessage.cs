// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageHasUpdateMessage.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NuGet.Versioning;

namespace ChocolateyGui.Common.Models.Messages
{
    public class PackageOutdatedMessage
    {
        public PackageOutdatedMessage(string id, NuGetVersion version, ChocolateySource source)
        {
            Id = id;
            Version = version;
            Source = source;
        }

        public string Id { get; }

        public NuGetVersion Version { get; set; }

        public ChocolateySource Source { get; }
    }
}