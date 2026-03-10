namespace ChocolateyGui.Common.Models.Messages
{
    using NuGet.Versioning;

    public class PackageUpgradedMessage
    {
        public PackageUpgradedMessage(string id, NuGetVersion version, ChocolateySource source)
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