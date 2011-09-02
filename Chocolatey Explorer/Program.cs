using System;
using System.Windows.Forms;
using Chocolatey.Explorer.View;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PackageManager());
        }
    }
}
