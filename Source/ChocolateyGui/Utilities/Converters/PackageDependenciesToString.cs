using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace ChocolateyGui.Utilities.Converters
{
    public class PackageDependenciesToString : IValueConverter
    {
        private static readonly Regex PackageNameVersionRegex = new Regex(@"(?<Id>[\w\.]*):{1,2}(?<Version>[\w\.]*)");
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value as string))
                return "";

            var dependenciesString = (string)value;
            var dependencyStrings = dependenciesString.Split(new[] { '|' });
            var items = dependencyStrings.Select((dependency) =>
            {
                var result = "";

                var match = PackageNameVersionRegex.Match(dependency);
                var id = match.Groups["Id"];
                if (id == null || string.IsNullOrWhiteSpace(id.Value))
                    return result;

                result += id.Value;

                var version = match.Groups["Version"];
                if (version != null && !string.IsNullOrWhiteSpace(version.Value))
                {
                    result += " (" + version + ")";
                }
                return result;

            }).Where(dependecy => dependecy != "");

            return string.Join(", ", items);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
