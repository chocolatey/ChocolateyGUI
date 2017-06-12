// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="App.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using ChocolateyGui.Utilities;
using Serilog;

namespace ChocolateyGui
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        internal static SplashScreen SplashScreen { get; set; }

        [STAThread]
        public static void Main(string[] args)
        {
            var dpi = NativeMethods.GetScaleFactor();
            var img = "chocolatey.png";
            if (dpi >= 2f)
            {
                img = "chocolatey@3.png";
            }
            else if (dpi > 1.00f)
            {
                img = "chocolatey@2.png";
            }

            SplashScreen = new SplashScreen(img);
            SplashScreen.Show(true, true);

            var application = new App();
            application.InitializeComponent();
            try
            {
                application.Run();
            }
            catch (Exception ex)
            {
                if (Bootstrapper.IsExiting)
                {
                    Log.Logger?.Error(ex, "Exception propagated to root while shutting down.");
                    return;
                }

                throw;
            }
        }
    }
}