﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="App.xaml.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using Autofac;
using ChocolateyGui.Common.Enums;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Startup;
using ChocolateyGui.Common.Utilities;
using ChocolateyGui.Common.Windows;
using ChocolateyGui.Common.Windows.Startup;
using ChocolateyGui.Common.Windows.Theming;
using ChocolateyGui.Common.Windows.Utilities;

namespace ChocolateyGui
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly App _application = new App();
        private static readonly TranslationSource _translationSource = TranslationSource.Instance;

        #region DupFinder Exclusion
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);

                try
                {
                    if (string.Equals(requestedAssembly.Name, "chocolatey", StringComparison.OrdinalIgnoreCase))
                    {
                        var installDir = Environment.GetEnvironmentVariable("ChocolateyInstall");
                        if (string.IsNullOrEmpty(installDir))
                        {
                            var rootDrive = Path.GetPathRoot(Assembly.GetExecutingAssembly().Location);
                            if (string.IsNullOrEmpty(rootDrive))
                            {
                                return null; // TODO: Maybe return the chocolatey.dll file instead?
                            }

                            installDir = Path.Combine(rootDrive, "ProgramData", "chocolatey");
                        }

                        var assemblyLocation = Path.Combine(installDir, "choco.exe");

                        return AssemblyResolver.ResolveOrLoadAssembly("choco", string.Empty, assemblyLocation);
                    }

#if FORCE_CHOCOLATEY_OFFICIAL_KEY
                    var chocolateyGuiPublicKey = Bootstrapper.OfficialChocolateyGuiPublicKey;
#else
                    var chocolateyGuiPublicKey = Bootstrapper.UnofficialChocolateyGuiPublicKey;
#endif

                    if (AssemblyResolver.DoesPublicKeyTokenMatch(requestedAssembly, chocolateyGuiPublicKey)
                        && string.Equals(requestedAssembly.Name, Bootstrapper.ChocolateyGuiCommonAssemblySimpleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return AssemblyResolver.ResolveOrLoadAssembly(
                            Bootstrapper.ChocolateyGuiCommonAssemblySimpleName,
                            AssemblyResolver.GetPublicKeyToken(requestedAssembly),
                            Bootstrapper.ChocolateyGuiCommonAssemblyLocation);
                    }

                    if (AssemblyResolver.DoesPublicKeyTokenMatch(requestedAssembly, chocolateyGuiPublicKey)
                        && string.Equals(requestedAssembly.Name, Bootstrapper.ChocolateyGuiCommonWindowsAssemblySimpleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return AssemblyResolver.ResolveOrLoadAssembly(
                            Bootstrapper.ChocolateyGuiCommonWindowsAssemblySimpleName,
                            AssemblyResolver.GetPublicKeyToken(requestedAssembly),
                            Bootstrapper.ChocolateyGuiCommonWindowsAssemblyLocation);
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Possibly make these values translatable, do not use Resources directly, instead Use L(nameof(Resources.KEY_NAME));
                    var errorMessage = string.Format("Unable to load Chocolatey GUI assembly. {0}", ex.Message);
                    ChocolateyMessageBox.Show(errorMessage);
                    throw new ApplicationException(errorMessage);
                }

                return null;
            };

            InitializeComponent();
        }
        #endregion

        internal static SplashScreen SplashScreen { get; set; }

        [STAThread]
        public static void Main(string[] args)
        {
            var splashScreenService = Bootstrapper.Container.Resolve<ISplashScreenService>();
            splashScreenService.Show();

            _application.InitializeComponent();

            try
            {
                _application.Run();
            }
            catch (Exception ex)
            {
                if (Bootstrapper.IsExiting)
                {
                    Bootstrapper.Logger.Error(ex, L(nameof(Common.Properties.Resources.Command_GeneralError)));
                    return;
                }

                throw;
            }
        }

        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ThemeAssist.BundledTheme.Generate("ChocolateyGui");

            var configService = Bootstrapper.Container.Resolve<IConfigService>();
            var effectiveConfiguration = configService.GetEffectiveConfiguration();

            ThemeMode themeMode;
            if (effectiveConfiguration.DefaultToDarkMode == null)
            {
                themeMode = ThemeMode.WindowsDefault;
            }
            else if (effectiveConfiguration.DefaultToDarkMode.Value)
            {
                themeMode = ThemeMode.Dark;
            }
            else
            {
                themeMode = ThemeMode.Light;
            }

            if (string.IsNullOrEmpty(effectiveConfiguration.UseLanguage))
            {
                Internationalization.Initialize();
                configService.SetConfigValue(nameof(effectiveConfiguration.UseLanguage), CultureInfo.CurrentCulture.Name);
            }
            else
            {
                Internationalization.UpdateLanguage(effectiveConfiguration.UseLanguage);
            }

            ThemeAssist.BundledTheme.SyncTheme(themeMode);
        }

        private static string L(string key)
        {
            return _translationSource[key];
        }
    }
}