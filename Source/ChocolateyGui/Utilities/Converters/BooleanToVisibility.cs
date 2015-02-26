// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BooleanToVisibility.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public class BooleanToVisibility : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((value == null || (bool)value == false) ^ (parameter != null && bool.Parse((string)parameter))) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}