// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NullToVisibility.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public class NullToVisibility : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                return string.IsNullOrWhiteSpace((string)value) ? Visibility.Collapsed : Visibility.Visible;
            }

            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}