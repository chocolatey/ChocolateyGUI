using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Chocolatey.Explorer.Model;
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
            PackageGrid.Focus();
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
                if (_packagesService is ICacheable)
                {
                    ((ICacheable)_packagesService).InvalidateCache();
                }
                if (_packageVersionService is ICacheable)
                {
                    ((ICacheable)_packageVersionService).InvalidateCache();
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
                ClearStatus();
                var distinct = packages;
                if (packageTabControl.SelectedTab == tabInstalled)
                    distinct = packages.Reverse().Distinct().Reverse().ToList();
                lblStatus.Text = string.Format(strings.num_installed_packages, distinct.Count());
                PackageGrid.DataSource = distinct;
            }
        }

        private void VersionChangedHandler(PackageVersion version)
        {
            if (this.InvokeRequired)
            {
                Invoke(new PackageVersionHandler(VersionChangedHandler), new object[] { version });
            }
            else
            {
                packageVersionPanel.Version = version;
                btnUpdate.Enabled = version.CanBeUpdated;
                btnInstallUninstall.Checked = !version.IsInstalled;
                btnInstallUninstall.Enabled = true;
                ClearStatus();
                lblStatus.Text = string.Format(strings.num_packages, PackageGrid.Rows.Count);
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

        private void PackageGrid_SelectionChanged(object sender, EventArgs e)
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
            if (PackageGrid.SelectedRows.Count == 0) return;
            var selectedPackage = PackageGrid.SelectedRows[0].DataBoundItem as Package;
            DisableUserInteraction();
            SetStatus(string.Format(strings.updating_package, selectedPackage.Name));
            txtPowershellOutput.Visible = true;
            _packageService.UpdatePackage(selectedPackage.Name);
        }

        private void buttonInstallUninstall_Click(object sender, EventArgs e)
        {
            if (PackageGrid.SelectedRows.Count == 0) return;
            var selectedPackage = PackageGrid.SelectedRows[0].DataBoundItem as Package;
            InstallOrUninstallPackage(selectedPackage);
        }

        private void btnInstallUninstall_CheckStateChanged(object sender, EventArgs e)
        {
            UpdateInstallUninstallButtonLabel();
        }

        private void PackageGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 2 && e.RowIndex != -1) // isInstalled checkbox (not header)
            {
                var package = PackageGrid.SelectedRows[0].DataBoundItem as Package;
                InstallOrUninstallPackage(package);
            }
        }

        private void PackageGrid_KeyPress(object sender, KeyPressEventArgs e)
        {
            IEnumerable<int> charIndex = (PackageGrid.DataSource as IList<Package>)
                .Select((value, index) => new { value, index })
                .SkipWhile(pair => !pair.value.Name.StartsWith(e.KeyChar.ToString(), StringComparison.CurrentCultureIgnoreCase))
                .Select(result => result.index);

            if (charIndex.Count() > 0)
            {
                PackageGrid.CurrentCell = PackageGrid.Rows[charIndex.First()].Cells[0];
                PackageGrid.FirstDisplayedScrollingRowIndex = charIndex.First();
                PackageGrid.Rows[charIndex.First()].Selected = true;
            }
        }

        private void UpdateInstallUninstallButtonLabel()
        {
            if (btnInstallUninstall.Checked)
            {
                btnInstallUninstall.ImageIndex = 0;
                btnInstallUninstall.Text = strings.install;
            }
            else
            {
                btnInstallUninstall.ImageIndex = 1;
                btnInstallUninstall.Text = strings.uninstall;
            }
        }

        private void QueryPackageVersion()
        {
            if (PackageGrid.SelectedRows.Count == 0) return;
            var selectedPackage = PackageGrid.SelectedRows[0].DataBoundItem as Package;
            SetStatus(string.Format(strings.getting_package_information, selectedPackage.Name));
            EmptyTextBoxes();
            _packageVersionService.PackageVersion(selectedPackage.Name);
        }

        private void QueryAvailablePackges()
        {
            DisableUserInteraction();
            SetStatus(strings.getting_available_packages);
            packageTabControl.SelectedTab = tabAvailable;
            lblProgressbar.Style = ProgressBarStyle.Marquee;
            PackageGrid.DataSource = new List<Package>();
            _packagesService.ListOfPackages();
        }

        private void QueryInstalledPackages()
        {
            var settings = new Properties.Settings();
            var expandedLibDirectory = System.Environment.ExpandEnvironmentVariables(settings.ChocolateyLibDirectory);
            if (!System.IO.Directory.Exists(expandedLibDirectory))
            {
                MessageBox.Show(string.Format(strings.lib_dir_not_found, expandedLibDirectory));
            }
            else
            {
                DisableUserInteraction();
                SetStatus(strings.getting_installed_packages);
                packageTabControl.SelectedTab = tabInstalled;
                lblProgressbar.Style = ProgressBarStyle.Marquee;
                PackageGrid.DataSource = new List<Package>();
                _packagesService.ListOfInstalledPackages();
            }
        }

        private void InstallOrUninstallPackage(Package package)
        {
            if (package.IsInstalled)
            {
                var result = MessageBox.Show(this, 
                    string.Format(strings.really_uninstall_package_msg, package.Name), 
                    strings.really_uninstall_package_title, 
                    MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    DisableUserInteraction();
                    txtPowershellOutput.Visible = true;
                    SetStatus(string.Format(strings.uninstalling, package.Name));
                    _packageService.UninstallPackage(package.Name);
                }
            }
            else
            {
                DisableUserInteraction();
                txtPowershellOutput.Visible = true;
                SetStatus(string.Format(strings.installing, package.Name));
                _packageService.InstallPackage(package.Name);
            } 
        }

        private void EnableUserInteraction()
        {
            packageTabControl.Selected += packageTabControl_Selected;
            mainSplitContainer.Panel1.Enabled = true;
            tableLayoutPanel1.Enabled = true;
            mainMenu.Enabled = true;
            PackageGrid.Focus();
        }

        private void DisableUserInteraction()
        {
            packageTabControl.Selected -= packageTabControl_Selected;
            mainSplitContainer.Panel1.Enabled = false;
            tableLayoutPanel1.Enabled = false;
            mainMenu.Enabled = false;
            packageVersionPanel.LockPanel();
        }

        private void EmptyTextBoxes()
        {
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
