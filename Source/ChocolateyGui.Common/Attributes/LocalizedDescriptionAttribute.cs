// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalizedDescriptionAttribute.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using ChocolateyGui.Common.Properties;

namespace ChocolateyGui.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        public LocalizedDescriptionAttribute(string key)
            : base(Localize(key))
        {
        }

        private static string Localize(string key)
        {
            return Resources.ResourceManager.GetString(key);
        }
    }
}