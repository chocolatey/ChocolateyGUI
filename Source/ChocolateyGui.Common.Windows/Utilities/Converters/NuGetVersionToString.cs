// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NuGetVersionToString.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Windows.Utilities.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using chocolatey;
    using NuGet.Versioning;

    public class NuGetVersionToString : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NuGetVersion version)
            {
                return version.ToNormalizedStringChecked();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string sValue && NuGetVersion.TryParse(sValue, out var version))
            {
                return version;
            }

            throw new InvalidOperationException("The passed in value is not a parseable string version!");
        }
    }
}