using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace ChocolateyGui.Utilities.Converters
{
    public class PackUriToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var url = value as string;
            if (url == null)
            {
                return null;
            }

            if (!url.StartsWith("pack:"))
            {
                return null;
            }

            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return null;
            }

            var resource = Application.GetRemoteStream(uri);
            if (resource == null)
            {
                return null;
            }

            using (var reader = new StreamReader(resource.Stream))
            {
                return reader.ReadToEnd();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
