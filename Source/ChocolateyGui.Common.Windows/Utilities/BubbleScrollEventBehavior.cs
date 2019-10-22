// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BubbleScrollEventBehavior.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace ChocolateyGui.Common.Windows.Utilities
{
    /// <summary>
    /// The BubbleScrollEventBehavior behavior can be used to prevent the mousewheel scrolling on a scrollable control.
    /// The event will be bubble up to the parent control.
    /// This behavior can be prevent with the left Shift key.
    /// </summary>
    public class BubbleScrollEventBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
            AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;

            base.OnDetaching();
        }

        private static void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var uiElement = sender as UIElement;
            if (uiElement == null || Keyboard.IsKeyDown(Key.LeftShift))
            {
                return;
            }

            e.Handled = true;

            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) { RoutedEvent = UIElement.MouseWheelEvent };
            uiElement.RaiseEvent(e2);
        }
    }
}