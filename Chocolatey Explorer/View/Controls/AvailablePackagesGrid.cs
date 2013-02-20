using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.PackagesService;

namespace Chocolatey.Explorer.View.Controls
{
    public class AvailablePackagesGrid : PackagesBaseGrid
    {
        private IAvailablePackagesService _availablePackagesService;
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IAvailablePackagesService AvailablePackagesService
        {
            get { return _availablePackagesService; }
            set
            {
                _availablePackagesService = value;
                _availablePackagesService.RunFinshed += AvailablePackagesServiceRunFinished;
                _availablePackagesService.RunFailed += AvailablePackagesServiceRunFailed;
            }
        }

        private void AvailablePackagesServiceRunFailed(Exception exc)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => AvailablePackagesServiceRunFailed(exc)));
            }
            else
            {
                DataSource = null;
            }
        }

        private void AvailablePackagesServiceRunFinished(IList<Package> packages)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => AvailablePackagesServiceRunFinished(packages)));
            }
            else
            {
                IList<Package> distinct = packages.Distinct().ToList();
                DataSource = distinct;
            }
        }
    }
}