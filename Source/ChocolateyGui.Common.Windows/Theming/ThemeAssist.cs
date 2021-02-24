// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeAssist.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Media;
using Autofac;
using ChocolateyGui.Common.Windows.Services;
using ControlzEx.Theming;

namespace ChocolateyGui.Common.Windows.Theming
{
    public static class ThemeAssist
    {
        /// <summary>
        /// Gets or sets the base color scheme for the FrameworkElement.
        /// </summary>
        public static readonly DependencyProperty BaseColorSchemeProperty
            = DependencyProperty.RegisterAttached(
                "BaseColorScheme",
                typeof(string),
                typeof(ThemeAssist),
                new FrameworkPropertyMetadata(
                    ThemeManager.BaseColorLight,
                    OnBaseColorSchemePropertyChanged));

        /// <summary>
        /// Gets the bundled theme instance for this application.
        /// </summary>
        public static IBundledThemeService BundledTheme { get; } = Bootstrapper.Container.Resolve<IBundledThemeService>();

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static string GetBaseColorScheme(DependencyObject obj)
        {
            return (string)obj.GetValue(BaseColorSchemeProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetBaseColorScheme(DependencyObject obj, string value)
        {
            obj.SetValue(BaseColorSchemeProperty, value);
        }

        /// <summary>
        /// Creates a freezed <see cref="SolidColorBrush"/> from the given <see cref="Color"/>.
        /// </summary>
        /// <param name="color">The color for the <see cref="SolidColorBrush"/>.</param>
        /// <returns>The <see cref="SolidColorBrush"/> from the given <see cref="Color"/>.</returns>
        public static Brush ToBrush(this Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        /// <summary>
        /// Converts a color string to a <see cref="Color"/>. If the string couldn't parsed the result will be black.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>The converted <see cref="Color"/> from the string.</returns>
        public static Color ColorFromString(string value)
        {
            return ColorConverter.ConvertFromString(value) is Color color ? color : Colors.Black;
        }

        private static void OnBaseColorSchemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement frameworkElement && e.OldValue != e.NewValue && e.NewValue is string baseColorScheme)
            {
                ThemeManager.Current.ChangeThemeBaseColor(frameworkElement, baseColorScheme);
            }
        }
    }
}