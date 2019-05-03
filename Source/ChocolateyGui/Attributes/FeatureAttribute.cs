// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="FeatureAttribute.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ChocolateyGui.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class FeatureAttribute : Attribute
    {
    }
}