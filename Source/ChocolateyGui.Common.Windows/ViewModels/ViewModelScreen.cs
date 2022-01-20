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