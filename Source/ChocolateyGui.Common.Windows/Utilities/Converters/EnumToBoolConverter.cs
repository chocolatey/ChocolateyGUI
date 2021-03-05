// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="EnumToBoolConverter.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;

namespace ChocolateyGui.Common.Windows.Utilities.Converters
{
    public class EnumToBoolConverter : IValueConverter
    {
        public static object DefaultEnumValue(Type enumType)
        {
            if (enumType != null)
            {
                if (enumType.IsEnum)
                {
                    return Enum.GetValues(enumType).GetValue(0);
                }

                throw new ArgumentException("given type is not an enum");
            }

            return null;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum valueEnum && parameter is Enum paramEnum)
            {
                var equal = System.Convert.ToInt64(paramEnum) == 0 ? System.Convert.ToInt64(valueEnum) == 0 : valueEnum.HasFlag(paramEnum);
                return equal;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(true, value) ? parameter : ((parameter != null) ? DefaultEnumValue(parameter.GetType()) : Binding.DoNothing);
        }
    }
}