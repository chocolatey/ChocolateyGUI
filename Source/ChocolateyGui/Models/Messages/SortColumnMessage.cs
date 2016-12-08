// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SortColumnMessage.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models.Messages
{
    public class SortColumnMessage
    {
        public SortColumnMessage(string name, bool sortDescending)
        {
            Name = name;
            SortDescending = sortDescending;
        }

        public string Name { get; }

        public bool SortDescending { get; }
    }
}
