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
                SelectionChanged -= GridSelectionChanged; 
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
                SelectionChanged += GridSelectionChanged;
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
                DataSource = packages;
                SelectionChanged += GridSelectionChanged;
            }
        }
    }
}