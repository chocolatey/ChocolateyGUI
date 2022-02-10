// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ViewModelScreen.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Caliburn.Micro;
using ChocolateyGui.Common.Utilities;

namespace ChocolateyGui.Common.Windows.ViewModels
{
    public abstract class ViewModelScreen : Screen
    {
        private readonly TranslationSource _translationSource;

        protected ViewModelScreen()
            : this(TranslationSource.Instance)
        {
        }

        protected ViewModelScreen(TranslationSource translationSource)
        {
            _translationSource = translationSource;
            _translationSource.PropertyChanged += (sender, args) => OnLanguageChanged();
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