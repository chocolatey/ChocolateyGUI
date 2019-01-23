// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalizedDescriptionAttribute.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using ChocolateyGui.Properties;

namespace ChocolateyGui.Attributes
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
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