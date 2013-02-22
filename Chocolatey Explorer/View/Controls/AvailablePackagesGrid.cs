using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Chocolatey.Explorer.Extensions;
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
            this.Invoke(() =>
                {
                    _bindingsource.DataSource = null;
                });
        }

        private void AvailablePackagesServiceRunFinished(IList<Package> packages)
        {
            this.Invoke(() =>
                {
                    IList<Package> distinct = packages.Distinct().ToList();
                    _bindingsource.DataSource = distinct;
                });
        }
    }
}