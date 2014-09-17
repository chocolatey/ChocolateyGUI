using System;
using System.Windows.Controls;

namespace ChocolateyGui.Services
{
    public interface INavigationService
    {
        /// <summary>
        /// Sets the Navigation Service's frame.
        /// </summary>
        /// <param name="frame">The frame used for navigation.</param>
        void SetNavigationItem(ContentControl frame);
        /// <summary>
        /// Navigates to a page of the specified type.
        /// </summary>
        /// <param name="pageType">The type of the page to be navigated to.</param>
        void Navigate(Type pageType);
        /// <summary>
        /// Navigates to a page of the specified type using the specified arguments for the page's constructor.
        /// </summary>
        /// <param name="pageType">The type of the page to be navigated to.</param>
        /// <param name="args">The <see cref="pageType"/> constructor's paramters. Each paramter must be uniquely typed.</param>
        void Navigate(Type pageType, params object[] args);
        /// <summary>
        /// Navigates to the specified page.
        /// </summary>
        /// <param name="page">The page to navigate to.</param>
        void Navigate(object page);
        /// <summary>
        /// Goes to the first page in the navigation stack.
        /// </summary>
        void GoHome();
        /// <summary>
        /// Goes back a single page in the navigation stack.
        /// </summary>
        void GoBack();
        /// <summary>
        /// Goes forward a single page in the navigation stack.
        /// </summary>
        void GoForward();
        /// <summary>
        /// Clears the navigation stack.
        /// </summary>
        void ClearNavigationStack();
        /// <summary>
        /// Navigates to a page of the specified type and clears the navigation stack.
        /// </summary>
        /// <param name="pageType">The type of the page to be navigated to.</param>
        void SetHome(Type pageType);
        /// <summary>
        /// Navigates to the specified and clears the navigation stack.
        /// </summary>
        /// <param name="page">The page to be navigated to.</param>
        void SetHome(object page);
        /// <summary>
        /// Whether there is a previous page on the navigation stack.
        /// </summary>
        bool CanGoBack { get; }
        /// <summary>
        /// Whether there is a next page on the navigation stack.
        /// </summary>
        bool CanGoForward { get; }
    }
}
