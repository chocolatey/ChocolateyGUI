﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Properties;
using Chocolatey.Explorer.Services;
using log4net;

namespace Chocolatey.Explorer.View
{
    public partial class PackageManager : Form,IPackageManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PackageManager));

        private delegate void PackageVersionHandler(PackageVersion version);
        private delegate void PackageSServiceHandler(IList<Package> packages);
        private delegate void PackageServiceHandler(string line);
        private delegate void PackageServiceRunFinishedHandler();
        private readonly IPackagesService _packagesService;
        private readonly IPackageVersionService _packageVersionService;
        private readonly IPackageService _packageService;

        public PackageManager(): this(new PackagesService(),new PackageVersionService(),new PackageService())
        {
        }

        public PackageManager(IPackagesService packagesService, IPackageVersionService packageVersionService, IPackageService packageService)
        {
            InitializeComponent();

            _packageService = packageService;
            _packagesService = packagesService;
            _packageVersionService = packageVersionService;
            _packageVersionService.VersionChanged += VersionChangedHandler;
            _packagesService.RunFinshed += PackagesServiceRunFinished;
            _packageService.LineChanged += PackageServiceLineChanged;
            _packageService.RunFinshed += PackageServiceRunFinished;
            ClearStatus();
            UpdateInstallUninstallButtonLabel();
            QueryInstalledPackages();
        }

        private void PackageServiceRunFinished()
        {
            if (this.InvokeRequired)
            {
                Invoke(new PackageServiceRunFinishedHandler(PackageServiceRunFinished));
            }
            else
            {
                EnableUserInteraction();
                ClearStatus();
                txtPowershellOutput.Visible = false;

                // invalidate caches, because package has been installed
                if (_packagesService.GetType() == typeof(CachedPackagesService))
                {
                    ((CachedPackagesService)_packagesService).InvalidateInstalledPackagesCache();
                }
                if (_packageVersionService.GetType() == typeof(CachedPackageVersionService))
                {
                    ((CachedPackageVersionService)_packageVersionService).InvalidateCache();
                }

                QueryInstalledPackages();
            }
        }

        private void PackageServiceLineChanged(string line)
        {
            if (this.InvokeRequired)
            {
                Invoke(new PackageServiceHandler(PackageServiceLineChanged), new object[] { line });
            }
            else
            {
                txtPowershellOutput.AppendText(line + Environment.NewLine);
            }
        }

        private void PackagesServiceRunFinished(IList<Package> packages)
        {
            if (this.InvokeRequired)
            {
                Invoke(new PackageSServiceHandler(PackagesServiceRunFinished), new object[] { packages });
            }
            else
            {
                EnableUserInteraction();
                var distinctpackages = packages.Distinct().ToList();
                PackageList.DataSource = distinctpackages;
                PackageList.DisplayMember = "Name";
                if(distinctpackages.Count > 0) PackageList.SelectedIndex = 0;
                ClearStatus();
                lblStatus.Text = "Number of installed packages: " + packages.Count;
                QueryPackageVersion(); 
            }
        }

        private void VersionChangedHandler(PackageVersion version)
        {
            if(this.InvokeRequired)
            {
                Invoke(new PackageVersionHandler(VersionChangedHandler), new object[] { version });
            }
            else
            {
                EnableUserInteraction();
                txtVersion.Text = "";
                txtVersion.AppendText(version.Name + Environment.NewLine);
                txtVersion.Select(0, version.Name.Length);
                txtVersion.SelectionFont = new Font(txtVersion.SelectionFont.FontFamily, 12, FontStyle.Bold);
                txtVersion.AppendText("Current version: " + version.CurrentVersion + Environment.NewLine);
                txtVersion.AppendText("Version on the server: " + version.Serverversion + Environment.NewLine);
                btnUpdate.Enabled = version.CanBeUpdated;
                btnInstallUninstall.Checked = !version.IsInstalled;
                btnInstallUninstall.Enabled = true;
                ClearStatus();
                lblStatus.Text = "Number of packages: " + PackageList.Items.Count;
            }
        }

        private void availablePackages_Click(object sender, EventArgs e)
        {
            QueryAvailablePackges();
        }

        private void installedPackages_Click(object sender, EventArgs e)
        {
            QueryInstalledPackages();
        }

        private void help_Click(object sender, EventArgs e)
        {
            var help = new Help();
            help.ShowDialog();
        }

        private void about_Click(object sender, EventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        private void settings_Click(object sender, EventArgs e)
        {
            var settings = new Settings();
            settings.ShowDialog();
        }

        private void packageList_MouseClick(object sender, MouseEventArgs e)
        {
            QueryPackageVersion();
        }

        private void packageTabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (packageTabControl.SelectedTab == tabAvailable)
            {
                QueryAvailablePackges();
            }
            else
            {
                QueryInstalledPackages();
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (PackageList.SelectedItem == null) return;
            DisableUserInteraction();
            SetStatus("Updating package " + ((Package)PackageList.SelectedItem).Name);
            txtPowershellOutput.Visible = true;
            _packageService.UpdatePackage(((Package) PackageList.SelectedItem).Name);
        }

        private void buttonInstallUninstall_Click(object sender, EventArgs e)
        {
            if (PackageList.SelectedItem == null) return;
            DisableUserInteraction();
            txtPowershellOutput.Visible = true;
            if (btnInstallUninstall.Checked)
            {
                SetStatus("Installing package " + ((Package)PackageList.SelectedItem).Name);
                _packageService.InstallPackage(((Package)PackageList.SelectedItem).Name);
            }
            else
            {
                SetStatus("Uninstalling package " + ((Package)PackageList.SelectedItem).Name);
                _packageService.UninstallPackage(((Package)PackageList.SelectedItem).Name);
            }
        }

        private void btnInstallUninstall_CheckStateChanged(object sender, EventArgs e)
        {
            UpdateInstallUninstallButtonLabel();
        }

        private void UpdateInstallUninstallButtonLabel()
        {
            if (btnInstallUninstall.Checked)
            {
                btnInstallUninstall.ImageIndex = 0;
                btnInstallUninstall.Text = "Install";
            }
            else
            {
                btnInstallUninstall.ImageIndex = 1;
                btnInstallUninstall.Text = "Uninstall";
            }
        }

        private void QueryPackageVersion()
        {
            if (PackageList.SelectedItem == null) return;
            DisableUserInteraction();
            SetStatus("Getting package information for package: " + ((Package)PackageList.SelectedItem).Name);
            EmptyTextBoxes();
            _packageVersionService.PackageVersion(((Package)PackageList.SelectedItem).Name);
        }

        private void QueryAvailablePackges()
        {
            DisableUserInteraction();
            SetStatus("Getting list of packages on server");
            packageTabControl.SelectedTab = tabAvailable;
            lblProgressbar.Style = ProgressBarStyle.Marquee;
            PackageList.DataSource = null;
            _packagesService.ListOfPackages();
        }

        private void QueryInstalledPackages()
        {
            var settings = new Properties.Settings();
            var expandedLibDirectory = System.Environment.ExpandEnvironmentVariables(settings.ChocolateyLibDirectory);
            if (!System.IO.Directory.Exists(expandedLibDirectory))
            {
                MessageBox.Show("Could not find the installed packages directory (" + expandedLibDirectory + "), please change the install directory in the settings.");
            }
            else
            {
                DisableUserInteraction();
                SetStatus("Getting list of installed packages");
                packageTabControl.SelectedTab = tabInstalled;
                lblProgressbar.Style = ProgressBarStyle.Marquee;
                PackageList.DataSource = null;
                _packagesService.ListOfInstalledPackages();
            }
        }

        private void EnableUserInteraction()
        {
            packageTabControl.Selected += packageTabControl_Selected;
            mainSplitContainer.Panel1.Enabled = true;
            tableLayoutPanel1.Enabled = true;
            mainMenu.Enabled = true;
        }

        private void DisableUserInteraction()
        {
            packageTabControl.Selected -= packageTabControl_Selected;
            mainSplitContainer.Panel1.Enabled = false;
            tableLayoutPanel1.Enabled = false;
            mainMenu.Enabled = false;
        }

        private void EmptyTextBoxes()
        {
            txtVersion.Text = "";
            txtPowershellOutput.Text = "";
            btnUpdate.Enabled = false;
            btnInstallUninstall.Enabled = false;
        }

        private void SetStatus(String text)
        {
            lblStatus.Text = text;
            lblProgressbar.Visible = true;
            lblProgressbar.Style = ProgressBarStyle.Marquee;
        }

        private void ClearStatus()
        {
            lblStatus.Text = "";
            lblProgressbar.Visible = false;
        }
    }
}
