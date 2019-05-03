// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalizedDescriptionAttribute.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using ChocolateyGui.Properties;

namespace ChocolateyGui.Attributes
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