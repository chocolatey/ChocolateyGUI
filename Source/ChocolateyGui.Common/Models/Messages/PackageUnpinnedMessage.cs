namespace ChocolateyGui.Common.Models.Messages
{
    using NuGet.Versioning;
    
    public class PackageUnpinnedMessage
    {
        public PackageUnpinnedMessage(string id, NuGetVersion version = null, ChocolateySource source = null)
        {
            Id = id;
            Version = version;
            Source = source;
        }

        public string Id { get; }

        public NuGetVersion Version { get; }

        public ChocolateySource Source { get; }
    }
}