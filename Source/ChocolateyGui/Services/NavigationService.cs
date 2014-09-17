using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Autofac;

namespace ChocolateyGui.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IProgressService _progressService;
        private readonly IComponentContext _pageFactory;
        public NavigationService(IProgressService progressService, IComponentContext pageFactory)
        {
            _progressService = progressService;
            _pageFactory = pageFactory;
        }

        private ContentControl _frame;
        public void SetNavigationItem(ContentControl frame)
        {
            if(frame == null)
                throw new ArgumentNullException("frame", @"Frame can't be null");
            _frame = frame;
        }

        public void Navigate(Type pageType)
        {
            var page = _pageFactory.Resolve(pageType);
            Navigate(page);
        }

        public void Navigate(Type pageType, params object[] args)
        {
            var parameters = args.Select(param => new TypedParameter(param.GetType(), param)).ToList();
            var page = _pageFactory.Resolve(pageType, parameters);
            Navigate(page);
        }

        public void Navigate(object page)
        {
            if(_frame.Content != null)
                _backStack.Push(_frame.Content);
            _frame.Content = page;
            _forwardStack.Clear();
        }

        public void GoHome()
        {
            while(CanGoBack)
               GoBack();
        }

        public void GoBack()
        {
            _forwardStack.Push(_frame.Content);
            _frame.Content = _backStack.Pop();
        }

        public void GoForward()
        {
            _backStack.Push(_frame.Content);
                _frame.Content = _forwardStack.Pop();
        }

        public void ClearNavigationStack()
        {
            _backStack.Clear();
            _forwardStack.Clear();
        }

        public void SetHome(Type pageType)
        {
            ClearNavigationStack();
            Navigate(pageType);
        }

        public void SetHome(object page)
        {
            ClearNavigationStack();
            Navigate(page);
        }

        public bool CanGoBack
        {
            get { return _backStack.Count > 0; }
        }

        public bool CanGoForward
        {
            get { return _forwardStack.Count > 0; }
        }

        private readonly Stack<object> _backStack = new Stack<object>();
        private readonly Stack<object> _forwardStack = new Stack<object>();
    }
}
