// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="StringListToString.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Data;

    public class StringListToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var items = value as IEnumerable<string>;
            if (items == null)
            {
                return string.Empty;
            }

            return string.Join(", ", items.Select(item => item.Trim()).ToList());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}