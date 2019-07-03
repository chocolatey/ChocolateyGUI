// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BooleanToVisibility.cs">
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
    public class BooleanToVisibility : DependencyObject, IValueConverter, IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return IsCollapsed(value, parameter)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var collapsed = values.Aggregate(false, (current, value) => current || IsCollapsed(value, parameter));
            return collapsed ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static bool IsCollapsed(object value, object parameter)
        {
            var boolVal = (value != null && value != DependencyProperty.UnsetValue) && (bool)value;
            return boolVal == false ^ (parameter != null && bool.Parse((string)parameter));
        }
    }
}