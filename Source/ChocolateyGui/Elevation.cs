// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Elevation.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Autofac;
using Caliburn.Micro;
using ChocolateyGui.Common;
using ChocolateyGui.Utilities.Extensions;

namespace ChocolateyGui
{
    public class Elevation : PropertyChangedBase
    {
        private bool _isElevated = Hacks.IsElevated;
        private bool _isBackgroundRunning = false;

        public static Elevation Instance
        {
            get { return Bootstrapper.Container.Resolve<Elevation>(); }
        }

        public bool IsElevated
        {
            get
            {
                return _isElevated;
            }

            set
            {
                this.SetPropertyValue(ref _isElevated, value);
                NotifyOfPropertyChange(nameof(CanDoCentralActions));
            }
        }

        public bool IsBackgroundRunning
        {
            get
            {
                return _isBackgroundRunning;
            }

            set
            {
                this.SetPropertyValue(ref _isBackgroundRunning, value);
                NotifyOfPropertyChange(nameof(CanDoCentralActions));
            }
        }

        public bool CanDoCentralActions
        {
            get { return _isBackgroundRunning || _isElevated; }
        }

        public bool CanDoTertiaryActions
        {
            get { return _isElevated; }
        }
    }
}