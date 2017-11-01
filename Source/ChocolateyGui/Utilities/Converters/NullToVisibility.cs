// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NullToVisibility.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;

namespace ChocolateyGui.Utilities.Converters
{
    public class NullToVisibility : NullToValue
    {
        public NullToVisibility()
        {
            TrueValue = Visibility.Collapsed;
            FalseValue = Visibility.Visible;
        }
    }
}