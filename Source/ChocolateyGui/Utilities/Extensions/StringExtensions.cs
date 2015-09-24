// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities.Extensions
{
    using System;

    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string source, string target)
        {
            return string.Compare(source, target, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
