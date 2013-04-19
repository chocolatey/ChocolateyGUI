using System;
using System.IO;
using System.Windows.Forms;
using Chocolatey.Explorer.IoC;
using Chocolatey.Explorer.View.Forms;
using StructureMap;

namespace Chocolatey.Explorer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LoggingExtensions.Logging.Log.InitializeWith<LoggingExtensions.NLog.NLogLog>();
            var startup = new Startup();
            startup.Start();
        }
    }
}
