// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToolTipBehavior.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace ChocolateyGui.Common.Windows.Utilities
{
    public class ToolTipBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty EnabledToolTipProperty
            = DependencyProperty.Register(
                nameof(EnabledToolTip),
                typeof(object),
                typeof(ToolTipBehavior),
                new PropertyMetadata(default, OnToolTipPropertyChanged));

        public static readonly DependencyProperty DisabledToolTipProperty
            = DependencyProperty.Register(
                nameof(DisabledToolTip),
                typeof(object),
                typeof(ToolTipBehavior),
                new PropertyMetadata(default, OnToolTipPropertyChanged));

        public static readonly DependencyProperty IsFeatureEnabledProperty
            = DependencyProperty.Register(
                nameof(IsFeatureEnabled),
                typeof(bool),
                typeof(ToolTipBehavior),
                new PropertyMetadata(true, OnToolTipPropertyChanged));

        public static readonly DependencyProperty DisabledFeatureToolTipProperty
            = DependencyProperty.Register(
                nameof(DisabledFeatureToolTip),
                typeof(object),
                typeof(ToolTipBehavior),
                new PropertyMetadata(default, OnToolTipPropertyChanged));

        public object EnabledToolTip
        {
            get { return GetValue(EnabledToolTipProperty); }
            set { SetValue(EnabledToolTipProperty, value); }
        }

        public object DisabledToolTip
        {
            get { return GetValue(DisabledToolTipProperty); }
            set { SetValue(DisabledToolTipProperty, value); }
        }

        public bool IsFeatureEnabled
        {
            get { return (bool)GetValue(IsFeatureEnabledProperty); }
            set { SetValue(IsFeatureEnabledProperty, value); }
        }

        public object DisabledFeatureToolTip
        {
            get { return GetValue(DisabledFeatureToolTipProperty); }
            set { SetValue(DisabledFeatureToolTipProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SetCurrentValue(ToolTipService.ShowOnDisabledProperty, true);

            SetTheToolTip();

            AssociatedObject.IsEnabledChanged += AssociatedObject_IsEnabledChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.IsEnabledChanged -= AssociatedObject_IsEnabledChanged;

            base.OnDetaching();
        }

        private static void OnToolTipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (d as ToolTipBehavior)?.SetTheToolTip();
            }
        }

        private void AssociatedObject_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetTheToolTip();
        }

        private void SetTheToolTip()
        {
            if (AssociatedObject is null)
            {
                return;
            }

            if (!AssociatedObject.IsEnabled && !IsFeatureEnabled)
            {
                AssociatedObject.SetCurrentValue(FrameworkElement.ToolTipProperty, DisabledFeatureToolTip);
            }
            else if (AssociatedObject.IsEnabled)
            {
                AssociatedObject.SetCurrentValue(FrameworkElement.ToolTipProperty, EnabledToolTip);
            }
            else
            {
                AssociatedObject.SetCurrentValue(FrameworkElement.ToolTipProperty, DisabledToolTip);
            }
        }
    }
}