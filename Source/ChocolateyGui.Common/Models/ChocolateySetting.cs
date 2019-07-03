// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateySetting.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Models
{
    public class ChocolateySetting
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }
    }
}