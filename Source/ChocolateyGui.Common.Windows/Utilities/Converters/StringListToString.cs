// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="StringListToString.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ChocolateyGui.Common.Windows.Utilities.Converters
{
    public class StringListToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string valueAsString)
            {
                return string.Join(", ", valueAsString.Split(new[] { " ", ",", ";", "|" }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim()));
            }

            return value is IEnumerable<string> items
                ? string.Join(", ", items.Select(item => item.Trim()))
                : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}