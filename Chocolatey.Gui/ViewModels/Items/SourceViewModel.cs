using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Chocolatey.Gui.Base;
using Chocolatey.Gui.Services;

namespace Chocolatey.Gui.ViewModels.Items
{
    public class SourceViewModel : ObservableBase
    {
        private readonly IPackageService _packageService;
        public SourceViewModel(IPackageService packageService)
        {
            _packageService = packageService;
        }

        public string Name { get; set; }

        public string Url { get; set; }

        public Type PageType { get; set; }

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
                    Content = App.Container.Resolve(PageType);
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
