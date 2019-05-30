// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ConfigAttribute.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ChocolateyGui.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ConfigAttribute : Attribute
    {
    }
}