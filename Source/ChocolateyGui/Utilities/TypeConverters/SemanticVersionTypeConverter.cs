// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SemanticVersionTypeConverter.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities.TypeConverters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using ChocolateyGui.Models;

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