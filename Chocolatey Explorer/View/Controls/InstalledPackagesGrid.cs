using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.PackagesService;

namespace Chocolatey.Explorer.View.Controls
{
    public class InstalledPackagesGrid : PackagesBaseGrid
    {
        private IInstalledPackagesService _installedPackagesService;
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IInstalledPackagesService InstalledPackagesService
        {
            get { return _installedPackagesService; }
            set
            {
                _installedPackagesService = value;
                _installedPackagesService.RunFinshed += InstalledPackagesServiceRunFinished;
                _installedPackagesService.RunFailed += InstalledPackagesServiceRunFailed;
                Enabled = false;
            }
        }

        private void InstalledPackagesServiceRunFailed(Exception exc)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => InstalledPackagesServiceRunFailed(exc)));
            }
            else
            {
                _bindingsource.DataSource = null;
            }
        }

        private void InstalledPackagesServiceRunFinished(IList<Package> packages)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => InstalledPackagesServiceRunFinished(packages)));
            }
            else
            {
                
                _bindingsource.DataSource = packages;
            }
        }
    }
}