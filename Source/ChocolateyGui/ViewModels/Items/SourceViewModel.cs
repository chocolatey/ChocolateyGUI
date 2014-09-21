// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourceViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Items
{
    using ChocolateyGui.Base;

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