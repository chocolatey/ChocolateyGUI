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
        private string _name;
        private string _url;

        public string Name
        {
            get { return this._name; }
            set { this.SetPropertyValue(ref this._name, value); }
        }

        public string Url
        {
            get { return this._url; }
            set { this.SetPropertyValue(ref this._url, value); }
        }
    }
}