using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.Commands;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.ChocolateyService;
using Chocolatey.Explorer.Services.FileStorageService;
using Chocolatey.Explorer.Services.PackageService;
using Chocolatey.Explorer.Services.PackageVersionService;
using Chocolatey.Explorer.Services.PackagesService;
using Chocolatey.Explorer.Services.SettingsService;

namespace Chocolatey.Explorer.View.Forms
{
    public partial class PackageManager : Form,IPackageManager
    {
        private delegate void PackageVersionHandler(PackageVersion version);
        private delegate void PackageSServiceHandler(IList<Package> packages);
        private delegate void PackageServiceHandler(string line);
        private delegate void PackageServiceRunFinishedHandler();
        private readonly IAvailablePackagesService _availablePackagesService;
        private readonly IInstalledPackagesService _installedPackagesService;
        private readonly IPackageVersionService _packageVersionService;
        private readonly IPackageService _packageService;
		private readonly IFileStorageService _fileStorageService;
        private readonly ICommandExecuter _commandExecuter;
        private readonly ISettingsService _settingsService;

        public PackageManager(): this(new AvailablePackagesService(),new PackageVersionService(),new PackageService(), new LocalFileSystemStorageService(), new CommandExecuter(), new SettingsService(), new InstalledPackagesService(new ChocolateyLibDirHelper()))
        {
        }

        public PackageManager(IAvailablePackagesService availablePackagesService, IPackageVersionService packageVersionService, IPackageService packageService, IFileStorageService fileStorageService, ICommandExecuter commandExecuter, ISettingsService settingsService, IInstalledPackagesService installedPackagesService)
        {
            InitializeComponent();

            _packageService = packageService;
            _availablePackagesService = availablePackagesService;
            _packageVersionService = packageVersionService;
			_fileStorageService = fileStorageService;
            _commandExecuter = commandExecuter;
            _settingsService = settingsService;
            _installedPackagesService = installedPackagesService;
            _packageVersionService.VersionChanged += VersionChangedHandler;
            _availablePackagesService.RunFinshed += AvailablePackagesServiceRunFinished;
            _installedPackagesService.RunFinshed += AvailablePackagesServiceRunFinished;
            _packageService.LineChanged += PackageServiceLineChanged;
            _packageService.RunFinshed += PackageServiceRunFinished;
			_availablePackagesService.RunFailed += AvailablePackagesServiceRunFailed;
            _installedPackagesService.RunFailed += AvailablePackagesServiceRunFailed;
            ClearStatus();
            PackageGrid.Focus();
            UpdateInstallUninstallButtonLabel();
            QueryInstalledPackages();
            tabAvailable.ImageIndex = 0;
            tabInstalled.ImageIndex = 1;
        }

        private void PackageServiceRunFinished()
        {
            if (InvokeRequired)
            {
                Invoke(new PackageServiceRunFinishedHandler(PackageServiceRunFinished));
            }
            else
            {
                EnableUserInteraction();
                ClearStatus();
                txtPowershellOutput.Visible = false;

                // invalidate caches, because package has been installed
                if (_availablePackagesService is ICacheable)
                {
                    ((ICacheable)_availablePackagesService).InvalidateCache();
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
            if (InvokeRequired)
            {
                Invoke(new PackageServiceHandler(PackageServiceLineChanged), new object[] { line });
            }
            else
            {
                txtPowershellOutput.AppendText(line + Environment.NewLine);
            }
        }

        private void AvailablePackagesServiceRunFinished(IList<Package> packages)
        {
            if (InvokeRequired)
            {
                Invoke(new PackageSServiceHandler(AvailablePackagesServiceRunFinished), new object[] { packages });
            }
            else
            {
                EnableUserInteraction();
                ClearStatus();
                var distinct = packages;
                if (packageTabControl.SelectedTab == tabInstalled)
                    distinct = packages.Reverse().Distinct().Reverse().ToList();
                lblStatus.Text = string.Format(strings.num_installed_packages, distinct.Count()); 
                Activate();
                PackageGrid.DataSource = distinct;
            }
        }

		private void AvailablePackagesServiceRunFailed(Exception exc)
		{
			//TODO - should we do something to prevent them from using more of the app nd getting more errors?
			if (exc is ChocolateyVersionUnknownException || (exc is AggregateException || exc.InnerException is IChocolateyService))
			{
				var result = MessageBox.Show("Chocolatey version could not be detected. Either Chocolatey is not installed or we cannot access it.", "Chocolatey not found");
				Application.Exit();
			}
			else
				MessageBox.Show(String.Format("An unexpected error occurred (good thing you're technical, eh?)\n{0}: {1}", exc.GetType().Name, exc.Message), "Unexpected Application Error");
		}

        private void VersionChangedHandler(PackageVersion version)
        {
            if (InvokeRequired)
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
            _commandExecuter.Execute<OpenHelpCommand>();
        }

        private void about_Click(object sender, EventArgs e)
        {
            _commandExecuter.Execute<OpenAboutCommand>();
        }

        private void settings_Click(object sender, EventArgs e)
        {
            _commandExecuter.Execute<OpenSettingsCommand>();
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
                selectPackageGridRow(charIndex.First());
            }
        }

        private void UpdateInstallUninstallButtonLabel()
        {
            if (btnInstallUninstall.Checked)
            {
                btnInstallUninstall.ImageIndex = 0;
                btnInstallUninstall.Text = strings.install;
                btnInstallUninstall.AccessibleName = strings.install;
                btnInstallUninstall.AccessibleDescription = strings.install_long;
            }
            else
            {
                btnInstallUninstall.ImageIndex = 1;
                btnInstallUninstall.Text = strings.uninstall;
                btnInstallUninstall.AccessibleName = strings.uninstall;
                btnInstallUninstall.AccessibleDescription = strings.unsinstall_long;
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
            searchPackages.Text = "";
            SetStatus(strings.getting_available_packages);
            packageTabControl.SelectedTab = tabAvailable;
            lblProgressbar.Style = ProgressBarStyle.Marquee;
            PackageGrid.DataSource = new List<Package>();
            _availablePackagesService.ListOfAvalablePackages();
        }

        private void QueryInstalledPackages()
        {
            searchPackages.Text = "";
            var expandedLibDirectory = System.Environment.ExpandEnvironmentVariables(_settingsService.ChocolateyLibDirectory);
            if (!_fileStorageService.DirectoryExists(expandedLibDirectory))
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
				_installedPackagesService.ListOfIntalledPackages();
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
            buttonRow.Enabled = true;
            mainMenu.Enabled = true;
            PackageGrid.Focus();
        }

        private void DisableUserInteraction()
        {
            packageTabControl.Selected -= packageTabControl_Selected;
            mainSplitContainer.Panel1.Enabled = false;
            buttonRow.Enabled = false;
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

        private void searchPackages_TextChanged(object sender, EventArgs e)
        {
            DataGridViewRow rowFound = PackageGrid.Rows.OfType<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells.OfType<DataGridViewCell>()
                    .Any(cell => cell.ColumnIndex == 0 && ((String)cell.Value).StartsWith(searchPackages.Text, StringComparison.OrdinalIgnoreCase)));

            if (rowFound != null)
            {
                selectPackageGridRow(rowFound.Index);
            }
        }

        private void selectPackageGridRow(int rowIndex)
        {
            PackageGrid.Rows[rowIndex].Selected = true;
            PackageGrid.FirstDisplayedScrollingRowIndex = rowIndex;
            PackageGrid.CurrentCell = PackageGrid.Rows[rowIndex].Cells[0];
        }
    }
}
