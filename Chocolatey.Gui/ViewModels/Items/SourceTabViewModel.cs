using System;
using Autofac;
using Chocolatey.Gui.Base;

namespace Chocolatey.Gui.ViewModels.Items
{
    public class SourceTabViewModel : ObservableBase
    {
        private IComponentContext _componentContext;
        public SourceTabViewModel(IComponentContext componentContext, string name, Uri url, Type pageType)
        {
            _componentContext = componentContext;

            Name = name;
            Url = url;
            PageType = pageType;
        }

        public string Name { get; private set; }

        public Uri Url { get; private set; }

        public Type PageType { get; private set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetPropertyValue(ref _isSelected, value);

                if (_content != null)
                    return;

                Content = _componentContext.Resolve(PageType, new TypedParameter(typeof(Uri), Url));
                _componentContext = null;
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
