using ChocolateyGui.Base;

namespace ChocolateyGui.ViewModels.Items
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

        public void LoadContent()
        {
           Content = _lazyPage.Value;
        }

        private object _content;
        public object Content
        {
            get { return _content; }
            set { SetPropertyValue(ref _content, value); }
        }
    }
}
