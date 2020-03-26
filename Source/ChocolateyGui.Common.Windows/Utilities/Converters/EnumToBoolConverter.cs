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
            var enumVal = value as Enum;
            var paramVal = parameter as Enum;

            if (enumVal != null && paramVal != null)
            {
                var equal = System.Convert.ToInt64(paramVal) == 0 ? System.Convert.ToInt64(enumVal) == 0 : enumVal.HasFlag(paramVal);
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