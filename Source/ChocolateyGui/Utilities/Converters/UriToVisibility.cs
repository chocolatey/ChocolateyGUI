using System;
using System.Windows;
using System.Windows.Data;

namespace ChocolateyGui.Utilities.Converters
{
    public class UriToVisibility : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || (value is string && string.IsNullOrWhiteSpace((string)value)))
                return Visibility.Collapsed;
           
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
