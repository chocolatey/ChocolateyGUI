// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BundledThemeService.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ChocolateyGui.Common.Base;
using ChocolateyGui.Common.Enums;
using ChocolateyGui.Common.Windows.Commands;
using ChocolateyGui.Common.Windows.Theming;
using ControlzEx.Theming;

namespace ChocolateyGui.Common.Windows.Services
{
    public class BundledThemeService : ObservableBase, IBundledThemeService
    {
        private ThemeMode _themeMode;
        private bool _isLightTheme;
        private Theme _light;
        private Theme _dark;

        public BundledThemeService()
        {
            ToggleTheme =
                new RelayCommand(
                    o => { ThemeManager.Current.ChangeTheme(Application.Current, IsLightTheme ? Light : Dark); },
                    o => Light != null && Dark != null);
        }

        /// <inheritdoc />
        public ICommand ToggleTheme { get; }

        /// <inheritdoc />
        public Theme Light
        {
            get => _light;
            private set => SetPropertyValue(ref _light, value);
        }

        /// <inheritdoc />
        public Theme Dark
        {
            get => _dark;
            private set => SetPropertyValue(ref _dark, value);
        }

        /// <inheritdoc />
        public ThemeMode ThemeMode
        {
            get => _themeMode;
            private set => SetPropertyValue(ref _themeMode, value);
        }

        /// <inheritdoc />
        public bool IsLightTheme
        {
            get => _isLightTheme;
            set
            {
                if (SetPropertyValue(ref _isLightTheme, value))
                {
                    ThemeMode = value ? ThemeMode.Light : ThemeMode.Dark;
                }
            }
        }

        /// <inheritdoc />
        public void SyncTheme(ThemeMode mode)
        {
            if (mode == ThemeMode.WindowsDefault)
            {
                ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
                ThemeManager.Current.SyncTheme();

                var theme = ThemeManager.Current.DetectTheme();
                IsLightTheme = theme is null || theme.BaseColorScheme == ThemeManager.BaseColorLight;
            }
            else
            {
                IsLightTheme = mode == ThemeMode.Light;
                ThemeManager.Current.ChangeTheme(Application.Current, IsLightTheme ? Light : Dark);
            }

            ThemeMode = mode;
        }

        /// <inheritdoc />
        public void Generate(string scheme)
        {
            Light = ThemeManager.Current.AddTheme(GenerateTheme(scheme, false));
            Dark = ThemeManager.Current.AddTheme(GenerateTheme(scheme, true));

            if (Light != null)
            {
                ThemeManager.Current.ChangeTheme(Application.Current, Light);
            }

            IsLightTheme = true;
        }

        private static Theme GenerateTheme(string scheme, bool isDark)
        {
            var baseColor = isDark ? ThemeManager.BaseColorDark : ThemeManager.BaseColorLight;
            var accentColor = isDark ? ThemeAssist.ColorFromString("#FF4F6170") : ThemeAssist.ColorFromString("#FF202F3C");

            var theme = new Theme(
                name: $"{baseColor}.{scheme}",
                displayName: $"{scheme} theme ({baseColor})",
                baseColorScheme: baseColor,
                colorScheme: scheme,
                primaryAccentColor: accentColor,
                showcaseBrush: new SolidColorBrush(accentColor),
                isRuntimeGenerated: true,
                isHighContrast: false);

            var backgroundColor = isDark ? ThemeAssist.ColorFromString("#333333") : ThemeAssist.ColorFromString("#F0EEE0");
            theme.Resources[ChocolateyColors.BackgroundKey] = backgroundColor;
            theme.Resources[ChocolateyBrushes.BackgroundKey] = backgroundColor.ToBrush();

            var bodyColor = isDark ? ThemeAssist.ColorFromString("#F0EEE0") : ThemeAssist.ColorFromString("#333333");
            theme.Resources[ChocolateyColors.BodyKey] = bodyColor;
            theme.Resources[ChocolateyBrushes.BodyKey] = bodyColor.ToBrush();

            var outOfDateColor = ThemeAssist.ColorFromString("#b71c1c");
            theme.Resources[ChocolateyColors.OutOfDateKey] = outOfDateColor;
            theme.Resources[ChocolateyBrushes.OutOfDateKey] = outOfDateColor.ToBrush();

            theme.Resources[ChocolateyColors.OutOfDateForegroundKey] = Colors.White;
            theme.Resources[ChocolateyBrushes.OutOfDateForegroundKey] = Colors.White.ToBrush();

            var isInstalledColor = ThemeAssist.ColorFromString("#1b5e20");
            theme.Resources[ChocolateyColors.IsInstalledKey] = isInstalledColor;
            theme.Resources[ChocolateyBrushes.IsInstalledKey] = isInstalledColor.ToBrush();

            theme.Resources[ChocolateyColors.IsInstalledForegroundKey] = Colors.White;
            theme.Resources[ChocolateyBrushes.IsInstalledForegroundKey] = Colors.White.ToBrush();

            var preReleaseColor = ThemeAssist.ColorFromString("#ff8f00");
            theme.Resources[ChocolateyColors.PreReleaseKey] = preReleaseColor;
            theme.Resources[ChocolateyBrushes.PreReleaseKey] = preReleaseColor.ToBrush();

            theme.Resources[ChocolateyColors.PreReleaseForegroundKey] = Colors.Black;
            theme.Resources[ChocolateyBrushes.PreReleaseForegroundKey] = Colors.Black.ToBrush();

            return theme;
        }
    }
}