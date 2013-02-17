using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.PackagesService;

namespace Chocolatey.Explorer.View.Controls
{
    public class InstalledPackagesGrid : PackagesBaseGrid
    {
        private IInstalledPackagesService _installedPackagesService;
        

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IInstalledPackagesService InstalledPackagesService
        {
            get { return _installedPackagesService; }
            set
            {
                _installedPackagesService = value;
                _installedPackagesService.RunFinshed += InstalledPackagesServiceRunFinished;
                _installedPackagesService.RunFailed += InstalledPackagesServiceRunFailed;
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
                DataSource = null;
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
                IList<Package> distinct = packages.Distinct().ToList();
                DataSource = distinct;
            }
        }
    }
}