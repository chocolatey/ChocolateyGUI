// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageDependenciesToString.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities.Converters
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Data;

    public class PackageDependenciesToString : IValueConverter
    {
        private static readonly Regex PackageNameVersionRegex = new Regex(@"(?<Id>[\w\.]*):{1,2}(?<Version>[\w\.]*)");

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value as string))
            {
                return string.Empty;
            }

            var dependenciesString = (string)value;
            var dependencyStrings = dependenciesString.Split(new[] { '|' });
            var items = dependencyStrings.Select((dependency) =>
            {
                var result = string.Empty;

                var match = PackageNameVersionRegex.Match(dependency);
                var id = match.Groups["Id"];

                if (id == null || string.IsNullOrWhiteSpace(id.Value))
                {
                    return result;
                }

                result += id.Value;

                var version = match.Groups["Version"];

                if (version != null && !string.IsNullOrWhiteSpace(version.Value))
                {
                    result += " (" + version + ")";
                }

                return result;

            }).Where(dependecy => dependecy != string.Empty);

            return string.Join(", ", items);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}