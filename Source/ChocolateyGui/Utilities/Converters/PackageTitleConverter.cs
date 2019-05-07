// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageTitleConverter.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;
using NuGet;

namespace ChocolateyGui.Utilities.Converters
{
    public class PackageTitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var packageTitle = (string)values[0];
            var packageVersion = (SemanticVersion)values[1];
            var packageId = (string)values[2];
            var showAdditionalPackageInformation = (bool)values[3];

            return showAdditionalPackageInformation
                ? string.Format("{0} {1} ({2})", packageTitle, packageVersion, packageId)
                : string.Format("{0} {1}", packageTitle, packageVersion);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}