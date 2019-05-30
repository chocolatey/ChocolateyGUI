// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ElevationStatusProvider.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Caliburn.Micro;
using ChocolateyGui.Utilities.Extensions;

namespace ChocolateyGui.Providers
{
    public class ElevationStatusProvider : PropertyChangedBase
    {
        private bool _isElevated = Hacks.IsElevated;

        public bool IsElevated
        {
            get { return _isElevated; }
            set { this.SetPropertyValue(ref _isElevated, value); }
        }
    }
}