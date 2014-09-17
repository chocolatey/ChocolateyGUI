using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

// Full credit to the Nuget team for this implemenation!
using ChocolateyGui.Properties;

namespace ChocolateyGui.Models
{
    [TypeConverter(typeof (SemanticVersionTypeConverter))]
    [Serializable]
    public sealed class SemanticVersion : IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>
    {
        private static readonly Regex SemanticVersionRegex =
            new Regex("^(?<Version>\\d+(\\s*\\.\\s*\\d+){0,3})(?<Release>-[a-z][0-9a-z-]*)?$",
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        private static readonly Regex StrictSemanticVersionRegex =
            new Regex("^(?<Version>\\d+(\\.\\d+){2})(?<Release>-[a-z][0-9a-z-]*)?$",
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        private readonly string _originalString;

        public Version Version { get; private set; }

        public string SpecialVersion { get; private set; }

        static SemanticVersion()
        {
        }

        public SemanticVersion(string version)
            : this(SemanticVersion.Parse(version))
        {
            _originalString = version;
        }

        public SemanticVersion(int major, int minor, int build, int revision)
            : this(new Version(major, minor, build, revision))
        {
        }

        public SemanticVersion(int major, int minor, int build, string specialVersion)
            : this(new Version(major, minor, build), specialVersion)
        {
        }

        public SemanticVersion(Version version)
            : this(version, string.Empty)
        {
        }

        public SemanticVersion(Version version, string specialVersion)
            : this(version, specialVersion, null)
        {
        }

        private SemanticVersion(Version version, string specialVersion, string originalString)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            Version = NormalizeVersionValue(version);

            SpecialVersion = specialVersion ?? string.Empty;
            _originalString = string.IsNullOrEmpty(originalString)
                ? version + (!string.IsNullOrEmpty(specialVersion) ? "-" + specialVersion : null)
                : originalString;
        }

        internal SemanticVersion(SemanticVersion semVer)
        {
            _originalString = semVer.ToString();
            Version = semVer.Version;
            SpecialVersion = semVer.SpecialVersion;
        }

        public static bool operator ==(SemanticVersion version1, SemanticVersion version2)
        {
            if (ReferenceEquals(version1, version2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)version1 == null) || ((object)version2 == null))
            {
                return false;
            }
            return version1.Equals(version2);
        }

        public static bool operator !=(SemanticVersion version1, SemanticVersion version2)
        {
            return !(version1 == version2);
        }

        public static bool operator <(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1 == null)
                throw new ArgumentNullException("version1");
            
            return version1.CompareTo(version2) < 0;
        }

        public static bool operator <=(SemanticVersion version1, SemanticVersion version2)
        {
            if (!(version1 == version2))
                return version1 < version2;

            return true;
        }

        public static bool operator >(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1 == null)
                throw new ArgumentNullException("version1");

            return version2 < version1;
        }

        public static bool operator >=(SemanticVersion version1, SemanticVersion version2)
        {
            if (!(version1 == version2))
                return version1 > version2;

            return true;
        }

        public static SemanticVersion Parse(string version)
        {
            if (string.IsNullOrEmpty(version))
                throw new ArgumentException(Resources.Argument_cant_be_null_or_empty, "version");

            SemanticVersion semanticVersion;
            if (TryParse(version, out semanticVersion))
                return semanticVersion;

            throw new ArgumentException(
                string.Format(CultureInfo.CurrentCulture, Resources.InvalidVersionString, new object[1]
                {
                    version
                }), "version");
        }

        public static bool TryParse(string version, out SemanticVersion value)
        {
            return TryParseInternal(version, SemanticVersionRegex, out value);
        }

        public static bool TryParseStrict(string version, out SemanticVersion value)
        {
            return TryParseInternal(version, StrictSemanticVersionRegex, out value);
        }

        private static bool TryParseInternal(string version, Regex regex, out SemanticVersion semVer)
        {
            semVer = null;
            if (string.IsNullOrEmpty(version))
                return false;

            var match = regex.Match(version.Trim());
            Version result;
            if (!match.Success || !Version.TryParse(match.Groups["Version"].Value, out result))
                return false;

            semVer = new SemanticVersion(NormalizeVersionValue(result),
                match.Groups["Release"].Value.TrimStart(new []
                {
                    '-'
                }), version.Replace(" ", ""));
            return true;
        }

        public static SemanticVersion ParseOptionalVersion(string version)
        {
            SemanticVersion semanticVersion;
            TryParse(version, out semanticVersion);
            return semanticVersion;
        }

        private static Version NormalizeVersionValue(Version version)
        {
            return new Version(version.Major, version.Minor, Math.Max(version.Build, 0), Math.Max(version.Revision, 0));
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            var other = obj as SemanticVersion;
            if (other == null)
                throw new ArgumentException(Resources.TypeMustBeASemanticVersion, "obj");

            return CompareTo(other);
        }

        public int CompareTo(SemanticVersion other)
        {
            if (other == null)
                return 1;
            var num = Version.CompareTo(other.Version);
            if (num != 0)
                return num;
            var thisSvNull = string.IsNullOrEmpty(SpecialVersion);
            var otherSvNull = string.IsNullOrEmpty(other.SpecialVersion);
            if (thisSvNull && otherSvNull)
                return 0;
            if (thisSvNull)
                return 1;
            if (otherSvNull)
                return -1;
            return StringComparer.OrdinalIgnoreCase.Compare(SpecialVersion, other.SpecialVersion);
        }

        public override string ToString()
        {
            return _originalString;
        }

        public bool Equals(SemanticVersion other)
        {
            if (other != null && Version == other.Version)
                return SpecialVersion.Equals(other.SpecialVersion, StringComparison.OrdinalIgnoreCase);
            return false;
        }

        public override bool Equals(object obj)
        {
            var other = obj as SemanticVersion;
            return other != null && Equals(other);
        }

        private const long BaseCombinedHash64 = 5381L;
        private readonly Func<object, long, long> _addIntToHash = (obj, hashCode) => (hashCode << 5) + hashCode ^ (long)(obj != null ? obj.GetHashCode() : 0);
        public override int GetHashCode()
        {
            return _addIntToHash(SpecialVersion, _addIntToHash(Version, BaseCombinedHash64)).GetHashCode();
        }
    }

    public class SemanticVersionTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var version = value as string;
            SemanticVersion semanticVersion;
            if (version != null && SemanticVersion.TryParse(version, out semanticVersion))
                return semanticVersion;
            return null;
        }
    }
}
