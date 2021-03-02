// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="App.xaml.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Windows;
using Autofac;
using chocolatey;
using chocolatey.infrastructure.registration;
using ChocolateyGui.Common.Enums;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Windows;
using ChocolateyGui.Common.Windows.Theming;

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

#if FORCE_CHOCOLATEY_OFFICIAL_KEY
                var chocolateyGuiPublicKey = Bootstrapper.OfficialChocolateyPublicKey;
#else
                var chocolateyGuiPublicKey = Bootstrapper.UnofficialChocolateyPublicKey;
#endif

                try
                {
                    if (requestedAssembly.get_public_key_token().is_equal_to(chocolateyGuiPublicKey)
                        && requestedAssembly.Name.is_equal_to(Bootstrapper.ChocolateyGuiCommonAssemblySimpleName))
                    {
                        return AssemblyResolution.resolve_or_load_assembly(
                            Bootstrapper.ChocolateyGuiCommonAssemblySimpleName,
                            requestedAssembly.get_public_key_token(),
                            Bootstrapper.ChocolateyGuiCommonAssemblyLocation).UnderlyingType;
                    }

                    if (requestedAssembly.get_public_key_token().is_equal_to(chocolateyGuiPublicKey)
                        && requestedAssembly.Name.is_equal_to(Bootstrapper.ChocolateyGuiCommonWindowsAssemblySimpleName))
                    {
                        return AssemblyResolution.resolve_or_load_assembly(
                            Bootstrapper.ChocolateyGuiCommonWindowsAssemblySimpleName,
                            requestedAssembly.get_public_key_token(),
                            Bootstrapper.ChocolateyGuiCommonWindowsAssemblyLocation).UnderlyingType;
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = string.Format("Unable to load Chocolatey GUI assembly. {0}", ex.Message);
                    MessageBox.Show(errorMessage);
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