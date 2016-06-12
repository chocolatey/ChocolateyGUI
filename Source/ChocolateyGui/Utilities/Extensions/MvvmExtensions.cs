// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MvvmExtensions.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;
using Caliburn.Micro;

namespace ChocolateyGui.Utilities.Extensions
{
    internal static class MvvmExtensions
    {
        public static bool SetPropertyValue<T>(this PropertyChangedBase @base, ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(property, value))
            {
                return false;
            }

            property = value;
            @base.NotifyOfPropertyChange(propertyName);
            return true;
        }
    }
}
