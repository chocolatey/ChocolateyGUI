// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="MultiBooleanAndToVisibility.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ChocolateyGui.Common.Windows.Utilities.Converters
{
    public class MultiBooleanAndToVisibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var isPrerelease = (values.ElementAtOrDefault(0) as bool?).GetValueOrDefault();
            var showAdditionalPackageInformation = (values.ElementAtOrDefault(1) as bool?).GetValueOrDefault();

            return isPrerelease && showAdditionalPackageInformation ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}