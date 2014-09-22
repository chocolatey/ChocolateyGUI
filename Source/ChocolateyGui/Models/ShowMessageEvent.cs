// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ShowMessageEventArgs.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    using System;

    public delegate void ShowMessageEventHandler(object sender, ShowMessageEventArgs e);

    public class ShowMessageEventArgs : EventArgs
    {
        public string Title { get; set; }

        public string Message { get; set; }
    }
}