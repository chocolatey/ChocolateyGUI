using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.PackageService;
using Chocolatey.Explorer.Services.PackageVersionService;
using Chocolatey.Explorer.Services.PackagesService;
using StructureMap;
using Chocolatey.Explorer.Extensions;

namespace Chocolatey.Explorer.View.Controls
{
    public partial class Statusbar : UserControl
    {
        private IAvailablePackagesService _availablePackagesService;
        private IInstalledPackagesService _installedPackagesService;
        private IPackageVersionService _packageVersionService;
        private IPackageService _packageService;
        private int _numberOfInstalledPackages;
        private int _numberOfAvailablePackages;

        public Statusbar()
        {
            ObjectFactory.BuildUp(this);

            InitializeComponent();

            ClearStatus();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IInstalledPackagesService InstalledPackagesService
        {
            get { return _installedPackagesService; }
            set
            {
                _installedPackagesService = value;
                _installedPackagesService.RunFinshed += InstalledPackagesServiceRunFinished;
                _installedPackagesService.RunStarted += PackagesServiceRunStarted;
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IAvailablePackagesService AvailablePackagesService
        {
            get { return _availablePackagesService; }
            set
            {
                _availablePackagesService = value;
                _availablePackagesService.RunFinshed += AvailablePackagesServiceRunFinished;
                _availablePackagesService.RunStarted += PackagesServiceRunStarted;
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IPackageVersionService PackageVersionService
        {
            get { return _packageVersionService; }
            set
            {
                _packageVersionService = value;
                _packageVersionService.VersionChanged += VersionChangedHandler;
                _packageVersionService.RunStarted += PackageVersionServiceStarted;
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IPackageService PackageService
        {
            get { return _packageService; }
            set
            {
                _packageService = value;
                _packageService.RunFinshed += PackageServiceRunFinished;
                _packageService.RunStarted += PackageServiceRunStarted;
            }
        }

        private void InstalledPackagesServiceRunFinished(IList<Package> packages)
        {
            this.Invoke(() =>
               {
                   ClearStatus();
                   SetStatus();
                   _numberOfInstalledPackages = packages.Count;
               });
        }

        private void AvailablePackagesServiceRunFinished(IList<Package> packages)
        {
            this.Invoke(() =>
               {
                   ClearStatus();
                   SetStatus();
                   _numberOfAvailablePackages = packages.Count;
               });
        }

        private void PackageServiceRunStarted(string message)
        {
            this.Invoke(() =>
               {
                   lblprogress.Text = message;
                   progressbar1.Visible = true;
                   progressbar1.Style = ProgressBarStyle.Marquee;
               });
        }

        private void PackagesServiceRunStarted(string message)
        {
            this.Invoke(() =>
               {
                   lblprogress.Text = message;
                   progressbar1.Visible = true;
                   progressbar1.Style = ProgressBarStyle.Marquee;
                   SetStatus();
               });
        }

        private void PackageVersionServiceStarted(string message)
        {
            this.Invoke(() =>
               {
                   lblprogress.Text = message;
                   progressbar2.Visible = true;
                   progressbar2.Style = ProgressBarStyle.Marquee;
               });
        }

        private void PackageServiceRunFinished()
        {
            this.Invoke(() =>
               {
                   lblprogress.Text = "";
                   progressbar2.Visible = false;
               });
        }

        private void VersionChangedHandler(PackageVersion version)
        {
            this.Invoke(() =>
               {
                   lblprogress.Text = "";
                   progressbar2.Visible = false;
                   SetStatus();
               });
        }

        private void SetStatus()
        {
            this.Invoke(() =>
               {
                   lblStatus.Text = "Available packages: " + _numberOfAvailablePackages + " - Installed packages: " + _numberOfInstalledPackages;
               });
        }

        private void ClearStatus()
        {
            this.Invoke(() =>
                {
                    lblStatus.Text = "";
                    lblprogress.Text = "";
                    progressbar1.Visible = false;
                });
        }

    }
}
