// <copyright file="SemanticVersion.cs" company="Chocolatey">
//      Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

/* Copyright 2015 .NET Foundation

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
*/

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

#pragma warning disable SA1615 // Element return value must be documented
#pragma warning disable SA1202 // Elements must be ordered by access
#pragma warning disable SA1611 // Element parameters must be documented
#pragma warning disable SA1201 // Operator should not follow a method

namespace ChocolateyGui
{
    /// <summary>
    /// A hybrid implementation of SemVer that supports semantic versioning as described at http://semver.org while not strictly enforcing it to
    /// allow older 4-digit versioning schemes to continue working.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(SemanticVersionTypeConverter))]
    public sealed class SemanticVersion : IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>
    {
        private const RegexOptions Flags = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
        private static readonly Regex SemanticVersionRegex = new Regex(@"^(?<Version>\d+(\s*\.\s*\d+){0,3})(?<Release>-[a-z][0-9a-z-]*)?$", Flags);
        private static readonly Regex StrictSemanticVersionRegex = new Regex(@"^(?<Version>\d+(\.\d+){2})(?<Release>-[a-z][0-9a-z-]*)?$", Flags);
        private readonly string _originalString;

        public SemanticVersion(string version)
            : this(Parse(version))
        {
            // The constructor normalizes the version string so that it we do not need to normalize it every time we need to operate on it.
            // The original string represents the original form in which the version is represented to be used when printing.
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

        internal SemanticVersion(SemanticVersion semVer)
        {
            _originalString = semVer.ToString();
            Version = semVer.Version;
            SpecialVersion = semVer.SpecialVersion;
        }

        private SemanticVersion(Version version, string specialVersion, string originalString)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            Version = NormalizeVersionValue(version);
            SpecialVersion = specialVersion ?? string.Empty;
            _originalString = string.IsNullOrEmpty(originalString) ? version + (!string.IsNullOrEmpty(specialVersion) ? '-' + specialVersion : null) : originalString;
        }

        /// <summary>
        /// Gets the normalized version portion.
        /// </summary>
        public Version Version
        {
            get;
        }

        /// <summary>
        /// Gets the optional special version.
        /// </summary>
        public string SpecialVersion
        {
            get;
        }

        public string[] GetOriginalVersionComponents()
        {
            if (!string.IsNullOrEmpty(_originalString))
            {
                // search the start of the SpecialVersion part, if any
                var dashIndex = _originalString.IndexOf('-');
                var original = dashIndex != -1 ? _originalString.Substring(0, dashIndex) : _originalString;

                return SplitAndPadVersionString(original);
            }

            return SplitAndPadVersionString(Version.ToString());
        }

        private static string[] SplitAndPadVersionString(string version)
        {
            var a = version.Split('.');
            if (a.Length == 4)
            {
                return a;
            }

            // if 'a' has less than 4 elements, we pad the '0' at the end
            // to make it 4.
            var b = new[] { "0", "0", "0", "0" };
            Array.Copy(a, 0, b, 0, a.Length);
            return b;
        }

        /// <summary>
        /// Parses a version string using loose semantic versioning rules that allows 2-4 version components followed by an optional special version.
        /// </summary>
        public static SemanticVersion Parse(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentOutOfRangeException(nameof(version));
            }

            SemanticVersion semVer;
            if (!TryParse(version, out semVer))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid Version: {0}", version), nameof(version));
            }

            return semVer;
        }

        /// <summary>
        /// Parses a version string using loose semantic versioning rules that allows 2-4 version components followed by an optional special version.
        /// </summary>
        public static bool TryParse(string version, out SemanticVersion value)
        {
            return TryParseInternal(version, SemanticVersionRegex, out value);
        }

        /// <summary>
        /// Parses a version string using strict semantic versioning rules that allows exactly 3 components and an optional special version.
        /// </summary>
        public static bool TryParseStrict(string version, out SemanticVersion value)
        {
            return TryParseInternal(version, StrictSemanticVersionRegex, out value);
        }

        private static bool TryParseInternal(string version, Regex regex, out SemanticVersion semVer)
        {
            semVer = null;
            if (string.IsNullOrEmpty(version))
            {
                return false;
            }

            var match = regex.Match(version.Trim());
            Version versionValue;
            if (!match.Success || !Version.TryParse(match.Groups["Version"].Value, out versionValue))
            {
                return false;
            }

            semVer = new SemanticVersion(NormalizeVersionValue(versionValue), match.Groups["Release"].Value.TrimStart('-'), version.Replace(" ", string.Empty));
            return true;
        }

        /// <summary>
        /// Attempts to parse the version token as a SemanticVersion.
        /// </summary>
        /// <returns>An instance of SemanticVersion if it parses correctly, null otherwise.</returns>
        public static SemanticVersion ParseOptionalVersion(string version)
        {
            SemanticVersion semVer;
            TryParse(version, out semVer);
            return semVer;
        }

        private static Version NormalizeVersionValue(Version version)
        {
            return new Version(
                version.Major,
                version.Minor,
                Math.Max(version.Build, 0),
                Math.Max(version.Revision, 0));
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 1;
            }

            var other = obj as SemanticVersion;
            if (other == null)
            {
                throw new ArgumentException("Type must be a semantic version.", nameof(obj));
            }

            return CompareTo(other);
        }

        public int CompareTo(SemanticVersion other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }

            var result = Version.CompareTo(other.Version);

            if (result != 0)
            {
                return result;
            }

            var empty = string.IsNullOrEmpty(SpecialVersion);
            var otherEmpty = string.IsNullOrEmpty(other.SpecialVersion);
            if (empty && otherEmpty)
            {
                return 0;
            }

            if (empty)
            {
                return 1;
            }

            if (otherEmpty)
            {
                return -1;
            }

            return StringComparer.OrdinalIgnoreCase.Compare(SpecialVersion, other.SpecialVersion);
        }

        public static bool operator ==(SemanticVersion version1, SemanticVersion version2)
        {
            if (ReferenceEquals(version1, null))
            {
                return ReferenceEquals(version2, null);
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
            {
                throw new ArgumentNullException(nameof(version1));
            }

            return version1.CompareTo(version2) < 0;
        }

        public static bool operator <=(SemanticVersion version1, SemanticVersion version2)
        {
            return (version1 == version2) || (version1 < version2);
        }

        public static bool operator >(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1 == null)
            {
                throw new ArgumentNullException(nameof(version1));
            }

            return version2 < version1;
        }

        public static bool operator >=(SemanticVersion version1, SemanticVersion version2)
        {
            return (version1 == version2) || (version1 > version2);
        }

        public override string ToString()
        {
            return _originalString;
        }

        public bool Equals(SemanticVersion other)
        {
            return !ReferenceEquals(null, other) &&
                   Version.Equals(other.Version) &&
                   SpecialVersion.Equals(other.SpecialVersion, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            var semVer = obj as SemanticVersion;
            return !ReferenceEquals(null, semVer) && Equals(semVer);
        }

        public override int GetHashCode()
        {
            var hashCode = Version.GetHashCode();
            if (SpecialVersion != null)
            {
#pragma warning disable SA1407 // Arithmetic expressions must declare precedence
                hashCode = hashCode * 4567 + SpecialVersion.GetHashCode();
#pragma warning restore SA1407 // Arithmetic expressions must declare precedence
            }

            return hashCode;
        }
    }
}