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
                lblProgressbar.Style = ProgressBarStyle.Marquee;
                SetStatus(); 
                Activate();
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
                ClearStatus();
                SetStatus();
            }
        }

        private void HelpClick(object sender, EventArgs e)
        {
            _commandExecuter.Execute<OpenHelpCommand>();
        }

        private void AboutClick(object sender, EventArgs e)
        {
            _commandExecuter.Execute<OpenAboutCommand>();
        }

        private void SettingsClick(object sender, EventArgs e)
        {
            _commandExecuter.Execute<OpenSettingsCommand>();
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
        
        private void QueryAvailablePackges()
        {
            DisableUserInteraction();
            searchPackages.Text = "";
            SetStatus();
            packageTabControl.SelectedTab = tabAvailable;
            lblProgressbar.Visible = true; 
            lblProgressbar.Style = ProgressBarStyle.Marquee;
            _availablePackagesService.ListOfAvalablePackages();
        }

        private void QueryInstalledPackages()
        {
            searchPackages.Text = "";
            var expandedLibDirectory = Environment.ExpandEnvironmentVariables(_settingsService.ChocolateyLibDirectory);
            if (!_fileStorageService.DirectoryExists(expandedLibDirectory))
            {
                MessageBox.Show(string.Format(strings.lib_dir_not_found, expandedLibDirectory));
            }
            else
            {
                DisableUserInteraction();
                SetStatus();
                packageTabControl.SelectedTab = tabInstalled;
                lblProgressbar.Visible = true;
                lblProgressbar.Style = ProgressBarStyle.Marquee;
                _installedPackagesService.ListOfIntalledPackages();
            }
        }

        private void EnableUserInteraction()
        {
            packageTabControl.Selected += packageTabControl_Selected;
            mainSplitContainer.Panel1.Enabled = true;
            packageButtonsPanel1.Enabled = true;
            mainMenu.Enabled = true;
        }

        private void DisableUserInteraction()
        {
            packageTabControl.Selected -= packageTabControl_Selected;
            mainSplitContainer.Panel1.Enabled = false;
            packageButtonsPanel1.Enabled = false;
            mainMenu.Enabled = false;
            packageVersionPanel.LockPanel();
        }

        private void SetStatus()
        {
            lblStatus.Text = "Available packages: " + availablePackagesGrid1.RowCount + " - Installed packages: " + installedPackagesGrid1.RowCount;
        }

        private void ClearStatus()
        {
            lblStatus.Text = "";
            lblProgressbar.Visible = false;
        }

        private void searchPackages_TextChanged(object sender, EventArgs e)
        {
            DataGridViewRow rowFound = availablePackagesGrid1.Rows.OfType<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells.OfType<DataGridViewCell>()
                    .Any(cell => cell.ColumnIndex == 0 && ((String)cell.Value).StartsWith(searchPackages.Text, StringComparison.OrdinalIgnoreCase)));

            if (rowFound != null)
            {
                selectPackageGridRow(rowFound.Index);
            }
        }

        private void selectPackageGridRow(int rowIndex)
        {
            availablePackagesGrid1.Rows[rowIndex].Selected = true;
            availablePackagesGrid1.FirstDisplayedScrollingRowIndex = rowIndex;
            availablePackagesGrid1.CurrentCell = availablePackagesGrid1.Rows[rowIndex].Cells[0];
        }
    }
}
