// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SemanticVersionTypeConverter.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;
using NuGet;

namespace ChocolateyGui.Common.Models
{
    public class SemanticVersionTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var version = value as string;
            SemanticVersion semanticVersion;
            if (version != null && SemanticVersion.TryParse(version, out semanticVersion))
            {
                return semanticVersion;
            }

            return null;
        }
    }
}