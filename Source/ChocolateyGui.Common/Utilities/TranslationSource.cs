// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="TranslationSource.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using chocolatey;
using ChocolateyGui.Common.Properties;

namespace ChocolateyGui.Common.Utilities
{
    public sealed class TranslationSource : INotifyPropertyChanged
    {
        private static readonly Lazy<TranslationSource> _instance =
            new Lazy<TranslationSource>(() => new TranslationSource());

        private readonly ResourceManager _resourceManager = Resources.ResourceManager;
        private CultureInfo _currentCulture;

        public event PropertyChangedEventHandler PropertyChanged;

        public static TranslationSource Instance { get; } = _instance.Value;

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

        public string this[string key]
        {
            get
            {
                var value = _resourceManager.GetString(key, CurrentCulture);
#if DEBUG
                if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(key))
                {
                    return "[" + key + "]";
                }
#endif
                return value;
            }
        }

        public string this[string key, params object[] parameters]
        {
            get
            {
                var value = this[key];

                if (parameters != null && parameters.Length > 0)
                {
                    return string.Format(CurrentCulture, value, parameters);
                }

                return value;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}