using System;
using Chocolatey.Gui.Base;

namespace Chocolatey.Gui.ViewModels.Items
{
    public class SourceTabViewModel : ObservableBase
    {
        private readonly dynamic _lazyPage;
        public SourceTabViewModel(dynamic lazyPageContent, string name)
        {
            _lazyPage = lazyPageContent;

            Name = name;
        }

        public string Name { get; private set; }

        public Uri Url { get; private set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetPropertyValue(ref _isSelected, value);

                if (_content != null)
                    return;

                Content = _lazyPage.Value;
            }
        }

        private object _content;
        public object Content
        {
            get { return _content; }
            set { SetPropertyValue(ref _content, value); }
        }
    }
}
