// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BooleanToTickString.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public class BooleanToTickString : DependencyObject, IValueConverter
    {
        private const string Tick = "✓";

        private const string Cross = "✗";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Tick : Cross;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}