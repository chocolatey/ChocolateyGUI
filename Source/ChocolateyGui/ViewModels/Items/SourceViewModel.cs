// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourceViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Common.Base;

namespace ChocolateyGui.ViewModels.Items
{
    public class SourceViewModel : ObservableBase
    {
        private string _name;
        private string _url;

        public string Name
        {
            get { return _name; }
            set { SetPropertyValue(ref _name, value); }
        }

        public string Url
        {
            get { return _url; }
            set { SetPropertyValue(ref _url, value); }
        }
    }
}