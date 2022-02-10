// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NullToInverseBool.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Windows.Utilities.Converters
{
    public class NullToInverseBool : NullToValue
    {
        public NullToInverseBool()
        {
            TrueValue = false;
            FalseValue = true;
        }
    }
}