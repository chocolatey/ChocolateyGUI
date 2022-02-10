// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalizedDescriptionAttribute.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using ChocolateyGui.Common.Utilities;

namespace ChocolateyGui.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        public LocalizedDescriptionAttribute(string key)
            : base(Localize(key))
        {
            Key = key;
        }

        public string Key { get; set; }

        private static string Localize(string key)
        {
            return TranslationSource.Instance[key];
        }
    }
}