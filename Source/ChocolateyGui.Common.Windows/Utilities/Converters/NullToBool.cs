// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NullToBool.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Windows.Utilities.Converters
{
    public class NullToBool : NullToValue
    {
        public NullToBool()
        {
            TrueValue = true;
            FalseValue = false;
        }
    }
}