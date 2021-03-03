// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageDependenciesToString.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace ChocolateyGui.Common.Windows.Utilities.Converters
{
    public class PackageDependenciesToString : IValueConverter
    {
        private static readonly Regex PackageNameVersionRegex = new Regex(@"(?<Id>[\w\.]*):{1,2}(?<Version>[\w\.]*)");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dependenciesString = value as string;
            if (string.IsNullOrWhiteSpace(dependenciesString))
            {
                return string.Empty;
            }

            var dependencyStrings = dependenciesString.Split('|');
            var items = dependencyStrings
                .Select(dependency =>
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
                })
                .Where(dependecy => !string.IsNullOrEmpty(dependecy));

            return string.Join(", ", items);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}