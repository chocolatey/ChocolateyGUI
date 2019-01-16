// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="TranslationSource.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Resources;
using ChocolateyGui.Base;
using ChocolateyGui.Properties;

namespace ChocolateyGui.Utilities
{
    public sealed class TranslationSource : ObservableBase
    {
        private readonly ResourceManager resourceManager = Resources.ResourceManager;
        private CultureInfo currentCulture = null;

        public static TranslationSource Instance { get; } = new TranslationSource();

        public CultureInfo CurrentCulture
        {
            get
            {
                return currentCulture;
            }

            set
            {
                if (currentCulture != value)
                {
                    currentCulture = value;
                    NotifyPropertyChanged(string.Empty); // We call with an empty string on purpose to force an update for all resource strings.
                }
            }
        }

        public string this[string key] => resourceManager.GetString(key, CurrentCulture);
    }
}