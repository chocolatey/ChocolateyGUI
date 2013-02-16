using System;
using System.Windows.Forms;
using Chocolatey.Explorer.IoC;
using Chocolatey.Explorer.View.Forms;
using StructureMap;

namespace Chocolatey.Explorer
{
    public class Startup
    {
        public void Start()
        {
            this.Log().Info("Log4net initialized");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                ObjectFactory.Configure(configure => configure.AddRegistry<Registry>());
                this.Log().Info("Structuremap configured");
                this.Log().Info("Opening PackageManager");
                Application.Run((PackageManager)ObjectFactory.GetInstance<IPackageManager>());
            }
            catch (Exception ex)
            {
                this.Log().Fatal("Fatal exception: " + ex.Message, ex);
                throw;
            }
        }
    }
}