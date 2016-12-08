// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BooleanToVisibilityHidden.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChocolateyGui.Utilities.Converters
{
    public class BooleanToVisibilityHidden : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null || (bool) value == false) ^ (parameter != null && bool.Parse((string) parameter))
                ? Visibility.Hidden
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}