using System.Globalization;
using System.Windows.Data;
using ChocolateyGui.Common.Utilities;

namespace ChocolateyGui.Common.Windows.Utilities.Converters
{
    using System;

    /// <summary>
    /// A converter responsible for formatting strings that are localizable
    /// and accept a single value to use as the parameters.
    /// Nothing is done if the parameter do not exist.
    /// </summary>
    /// <notes>
    /// <para>
    /// The converter parameter is the string that contains the name of the resource
    /// to use as the format string.
    /// </para>
    /// <para>
    /// Be careful about the use of this converter, as dynamic language
    /// changes do not update the converter return value automatically.
    /// Additional steps needs to be taken to ensure it gets updated.
    /// </para>
    /// </notes>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    [ValueConversion(typeof(object), typeof(string), ParameterType = typeof(string))]
    public class LocalizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = "{0}";

            if (parameter is string sParameter)
            {
                format = string.Format(
                    TranslationSource.Instance[sParameter],
                    value);
            }

            return format;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}