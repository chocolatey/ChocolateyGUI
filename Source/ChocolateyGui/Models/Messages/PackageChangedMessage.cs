// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageChangedMessage.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using NuGet;

namespace ChocolateyGui.Models.Messages
{
    public enum PackageChangeType
    {
        Updated,
        Uninstalled,
        Installed,
        Pinned,
        Unpinned
    }

    public class PackageChangedMessage
    {
        public PackageChangedMessage(string id, PackageChangeType changeType, SemanticVersion version = null)
        {
            Id = id;
            ChangeType = changeType;
            Version = version;
        }

        public string Id { get; }

        public SemanticVersion Version { get; }

        public PackageChangeType ChangeType { get; }
    }
}