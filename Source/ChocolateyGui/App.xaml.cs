// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="App.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using Autofac;
using ChocolateyGui.Common.Services;

namespace ChocolateyGui
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly App _application = new App();

        public App()
        {
            InitializeComponent();
        }

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
    }
}