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

namespace ChocolateyGui.Utilities.Converters
{
    public class StringListToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = value as IEnumerable<string>;
            if (items == null)
            {
                return string.Empty;
            }

            return string.Join(", ", items.Select(item => item.Trim()).ToList());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}