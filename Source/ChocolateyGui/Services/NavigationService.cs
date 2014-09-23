// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NavigationService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;
    using Autofac;

    public class NavigationService : INavigationService
    {
        private readonly Stack<object> _backStack = new Stack<object>();
        private readonly Stack<object> _forwardStack = new Stack<object>();
        private readonly IComponentContext _pageFactory;
        private ContentControl _frame;

        public NavigationService(IComponentContext pageFactory)
        {
            this._pageFactory = pageFactory;
        }

        public bool CanGoBack
        {
            get { return this._backStack.Count > 0; }
        }

        public bool CanGoForward
        {
            get { return this._forwardStack.Count > 0; }
        }

        public void ClearNavigationStack()
        {
            this._backStack.Clear();
            this._forwardStack.Clear();
        }

        public void GoBack()
        {
            this._forwardStack.Push(this._frame.Content);
            this._frame.Content = this._backStack.Pop();
        }

        public void GoForward()
        {
            this._backStack.Push(this._frame.Content);
            this._frame.Content = this._forwardStack.Pop();
        }

        public void GoHome()
        {
            while (this.CanGoBack)
            {
                this.GoBack();
            }
        }

        public void Navigate(Type pageType)
        {
            var page = this._pageFactory.Resolve(pageType);
            this.Navigate(page);
        }

        public void Navigate(Type pageType, params object[] args)
        {
            var parameters = args.Select(param => new TypedParameter(param.GetType(), param)).ToList();
            var page = this._pageFactory.Resolve(pageType, parameters);
            this.Navigate(page);
        }

        public void Navigate(object page)
        {
            if (this._frame.Content != null)
            {
                this._backStack.Push(this._frame.Content);
            }

            this._frame.Content = page;
            this._forwardStack.Clear();
        }

        public void SetHome(Type pageType)
        {
            this.ClearNavigationStack();
            this.Navigate(pageType);
        }

        public void SetHome(object page)
        {
            this.ClearNavigationStack();
            this.Navigate(page);
        }

        public void SetNavigationItem(ContentControl frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame", @"Frame can't be null");
            }

            this._frame = frame;
        }
    }
}