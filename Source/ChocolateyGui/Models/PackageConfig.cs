using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ChocolateyGui.Models
{
    public class PackageConfig
    {
        public IEnumerable<PackageConfigEntry> Packages { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class PackageConfigEntry
    {
        public PackageConfigEntry(string id, SemanticVersion version, Uri source)
        {
            Id = id;
            Version = version;
            Source = source;
        }
        public string Id { get; private set; }
        public SemanticVersion Version { get; private set; }
        public Uri Source { get; private set; }
    }
}
