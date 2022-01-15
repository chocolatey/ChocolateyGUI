// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="App.xaml.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Autofac;
using ChocolateyGui.Common.Enums;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Startup;
using ChocolateyGui.Common.Windows;
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
                    var chocolateyGuiPublicKey = Bootstrapper.OfficialChocolateyPublicKey;
#else
                    var chocolateyGuiPublicKey = Bootstrapper.UnofficialChocolateyPublicKey;
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
                    Bootstrapper.Logger.Error(ex, Common.Properties.Resources.Command_GeneralError);
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
            var defaultToDarkMode = configService.GetEffectiveConfiguration().DefaultToDarkMode;

            ThemeMode themeMode;
            if (defaultToDarkMode == null)
            {
                themeMode = ThemeMode.WindowsDefault;
            }
            else if (defaultToDarkMode.Value)
            {
                themeMode = ThemeMode.Dark;
            }
            else
            {
                themeMode = ThemeMode.Light;
            }

            ThemeAssist.BundledTheme.SyncTheme(themeMode);
        }
    }
}