// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NullToVisibility.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;

namespace ChocolateyGui.Common.Windows.Utilities.Converters
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