using System;
using System.Windows.Forms;
using Chocolatey.Explorer.IoC;
using Chocolatey.Explorer.View;
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
            ObjectFactory.Configure(configure => configure.AddRegistry<Registry>());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ObjectFactory.GetInstance<IPackageManager>();
            Application.Run(new PackageManager());
        }
    }
}
