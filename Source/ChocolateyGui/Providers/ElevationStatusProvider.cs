// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ElevationStatusProvider.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using Caliburn.Micro;
using ChocolateyGui.Utilities;
using ChocolateyGui.Utilities.Extensions;

namespace ChocolateyGui.Providers
{
    public class ElevationStatusProvider : PropertyChangedBase
    {
        public static readonly ElevationStatusProvider Instance =
            (ElevationStatusProvider) Application.Current.FindResource("Elevation");

        private bool _isElevated = Hacks.IsElevated;

        private Tuple<bool, bool> _overrideElevation;

        public bool IsElevated
        {
            get
            {
                if (_overrideElevation.Item1)
                {
                    return _overrideElevation.Item2;
                }

                return _isElevated;
            }

            set
            {
                this.SetPropertyValue(ref _isElevated, value);
            }
        }

        public Tuple<bool, bool> OverrideElevation
        {
            get
            {
                return _overrideElevation;
            }

            set
            {
                _overrideElevation = value;
                NotifyOfPropertyChange(nameof(IsElevated));
            }
        }
    }
}