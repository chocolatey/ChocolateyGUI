using System;
using System.Linq;
using System.Windows.Controls;
using Autofac;

namespace Chocolatey.Gui.Services
{
    public class NavigationService : INavigationService
    {
        private Frame _frame;
        public void SetNavigationItem(Frame frame)
        {
            if(frame == null)
                throw new ArgumentNullException("frame", @"Frame can't be null");
            _frame = frame;
        }

        public void Navigate(Type pageType)
        {
            var page = App.Container.Resolve(pageType);
            Navigate(page);
        }

        public void Navigate(Type pageType, params object[] args)
        {
            var parameters = args.Select(param => new TypedParameter(param.GetType(), param)).ToList();
            var page = App.Container.Resolve(pageType, parameters);
            Navigate(page);
        }

        public void Navigate(object page)
        {
            _frame.Navigate(page);
        }

        public void GoHome()
        {
            while(CanGoBack)
               GoBack();
        }

        public void GoBack()
        {
            if(CanGoBack)
                _frame.GoBack();
        }

        public void GoForward()
        {
            if(CanGoForward)
                _frame.GoForward();
        }

        public void ClearNavigationStack()
        {
            throw new NotImplementedException();
        }

        public void SetHome(Type pageType)
        {
            GoHome();
            Navigate(pageType);
            _frame.RemoveBackEntry();
        }

        public void SetHome(object page)
        {
            GoHome();
            Navigate(page);
            _frame.RemoveBackEntry();
        }

        public bool CanGoBack
        {
            get { return _frame.CanGoBack; }
        }

        public bool CanGoForward
        {
            get { return _frame.CanGoForward; }
        }
    }
}
