// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ObservableBase.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChocolateyGui.Common.Utilities;

namespace ChocolateyGui.Common.Base
{
    public abstract class ObservableBase : INotifyPropertyChanged
    {
        private readonly TranslationSource _translationSource;

        protected ObservableBase()
            : this(TranslationSource.Instance)
        {
        }

        protected ObservableBase(TranslationSource translationSource)
        {
            _translationSource = translationSource;
            _translationSource.PropertyChanged += (sender, args) => OnLanguageChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool SetPropertyValue<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
            {
                return false;
            }

            property = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            var propertyChangedEvent = PropertyChanged;
            if (propertyChangedEvent != null)
            {
                propertyChangedEvent(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected string L(string key)
        {
            return _translationSource[key];
        }

        protected string L(string key, params object[] parameters)
        {
            return _translationSource[key, parameters];
        }

        protected virtual void OnLanguageChanged()
        {
        }
    }
}