// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackagesChangedEventArgs.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    using System;
    using ChocolateyGui.Enums;

    public delegate void PackagesChangedEventHandler(object sender, PackagesChangedEventArgs e);

    public class PackagesChangedEventArgs : EventArgs
    {
        public PackagesChangedEventType EventType { get; set; }

        public string PackageId { get; set; }

        public string PackageVersion { get; set; }
    }
}