using System;
using System.IO;
using System.Windows.Forms;
using Chocolatey.Explorer.IoC;
using Chocolatey.Explorer.View.Forms;
using StructureMap;
using log4net;

namespace Chocolatey.Explorer
{
    static class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
            log.Info("Log4net initialized");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                ObjectFactory.Configure(configure => configure.AddRegistry<Registry>());
                log.Info("Structuremap configured");
                log.Info("Opening PackageManager");
                Application.Run((PackageManager)ObjectFactory.GetInstance<IPackageManager>());
            }
            catch (Exception ex)
            {
                log.Fatal("Fatal exception: " + ex.Message, ex);
                throw;
            }
            
        }
    }
}
