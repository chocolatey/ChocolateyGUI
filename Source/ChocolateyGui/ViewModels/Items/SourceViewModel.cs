using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChocolateyGui.Base;

namespace ChocolateyGui.ViewModels.Items
{
    public class SourceViewModel : ObservableBase
    {
        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetPropertyValue(ref _url, value); }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetPropertyValue(ref _name, value); }
        }
    }
}
