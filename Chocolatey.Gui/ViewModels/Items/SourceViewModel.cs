using System;
using Autofac;
using Chocolatey.Gui.Base;
using Chocolatey.Gui.Services;

namespace Chocolatey.Gui.ViewModels.Items
{
    public class SourceViewModel : ObservableBase
    {
        private readonly IPackageService _packageService;
        private IComponentContext _componentContext;
        public SourceViewModel(IPackageService packageService, IComponentContext componentContext, string name, Uri url, Type pageType)
        {
            _packageService = packageService;
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
                if (Url != null)
                {
                    _packageService.SetSource(Url);
                }

                if (_content == null)
                {
                    Content = _componentContext.Resolve(PageType);
                    _componentContext = null;
                }
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
