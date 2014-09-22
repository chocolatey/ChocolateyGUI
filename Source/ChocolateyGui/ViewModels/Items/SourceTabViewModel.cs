// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourceTabViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Items
{
    using ChocolateyGui.Base;

    public class SourceTabViewModel : ObservableBase
    {
        private readonly dynamic _lazyPage;
        private object _content;

        public SourceTabViewModel(dynamic lazyPageContent, string name)
        {
            this._lazyPage = lazyPageContent;

            this.Name = name;
        }

        public object Content
        {
            get { return this._content; }
            set { this.SetPropertyValue(ref this._content, value); }
        }

        public string Name { get; private set; }

        public void LoadContent()
        {
            this.Content = this._lazyPage.Value;
        }
    }
}