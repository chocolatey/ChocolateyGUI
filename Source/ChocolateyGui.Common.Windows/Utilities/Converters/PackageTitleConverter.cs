// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageTitleConverter.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using NuGet;

namespace ChocolateyGui.Common.Windows.Utilities.Converters
{
    public class PackageTitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var packageTitle = values.ElementAtOrDefault(0);
            var packageVersion = values.ElementAtOrDefault(1) as SemanticVersion;
            var packageId = values.ElementAtOrDefault(2);
            var showAdditionalPackageInformation = (values.ElementAtOrDefault(3) as bool?).GetValueOrDefault();

            return showAdditionalPackageInformation
                ? $"{packageTitle} {packageVersion} ({packageId})"
                : $"{packageTitle} {packageVersion}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}