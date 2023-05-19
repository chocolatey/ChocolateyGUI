// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NuGetVersionTypeConverter.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;
using NuGet.Versioning;

namespace ChocolateyGui.Common.Models
{
    public class NuGetVersionTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var version = value as string;
            NuGetVersion semanticVersion;
            if (version != null && NuGetVersion.TryParse(version, out semanticVersion))
            {
                return semanticVersion;
            }

            return null;
        }
    }
}