// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBundledThemeService.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Input;
using ChocolateyGui.Common.Enums;
using ControlzEx.Theming;

namespace ChocolateyGui.Common.Windows.Services
{
    public interface IBundledThemeService
    {
        /// <summary>
        /// Gets the command to toggle between light and dark theme.
        /// </summary>
        ICommand ToggleTheme { get; }

        /// <summary>
        /// Gets the generated light theme for the application.
        /// </summary>
        Theme Light { get; }

        /// <summary>
        /// Gets the generated dark theme for the application.
        /// </summary>
        Theme Dark { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the current selected theme.
        /// </summary>
        ThemeMode ThemeMode { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the current selected theme is the light theme.
        /// </summary>
        bool IsLightTheme { get; set; }

        /// <summary>
        /// Syncs the application with the "app mode" setting from windows which should be detected at runtime and the current <see cref="T:ControlzEx.Theming.Theme" /> be changed accordingly.
        /// </summary>
        /// <param name="mode">The sync mode for this application.</param>
        void SyncTheme(ThemeMode mode);

        /// <summary>
        /// Generates the themes for this application.
        /// </summary>
        /// <param name="scheme">The base scheme for this application.</param>
        void Generate(string scheme);
    }
}