using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Chocolatey.Explorer.Extensions;
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
            this.Invoke(() =>
                {
                    _bindingsource.DataSource = null;
                });
        }

        private void InstalledPackagesServiceRunFinished(IList<Package> packages)
        {
            this.Invoke(() =>
                {
                    _bindingsource.DataSource = packages;
                });
        }
    }
}