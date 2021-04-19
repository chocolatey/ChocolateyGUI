// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="TranslationSource.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Resources;
using ChocolateyGui.Common.Base;
using ChocolateyGui.Common.Properties;

namespace ChocolateyGui.Common.Windows.Utilities
{
    public sealed class TranslationSource : ObservableBase
    {
        private readonly ResourceManager _resourceManager = Resources.ResourceManager;
        private CultureInfo _currentCulture;

        public static TranslationSource Instance { get; } = new TranslationSource();

        public CultureInfo CurrentCulture
        {
            get
            {
                return _currentCulture;
            }

            set
            {
                if (!Equals(_currentCulture, value))
                {
                    _currentCulture = value;
                    NotifyPropertyChanged(string.Empty); // We call with an empty string on purpose to force an update for all resource strings.
                }
            }
        }

        public string this[string key] => _resourceManager.GetString(key, CurrentCulture);
    }
}