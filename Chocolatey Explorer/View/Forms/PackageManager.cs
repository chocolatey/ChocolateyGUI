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
        private delegate void PackageSServiceHandler(IList<Package> packages);
        private delegate void PackageServiceRunFinishedHandler();
        private readonly IAvailablePackagesService _availablePackagesService;
        private readonly IInstalledPackagesService _installedPackagesService;
        private readonly IPackageVersionService _packageVersionService;
        private readonly IPackageService _packageService;
		private readonly IFileStorageService _fileStorageService;
        private readonly ICommandExecuter _commandExecuter;
        private readonly ISettingsService _settingsService;

        public PackageManager(IAvailablePackagesService availablePackagesService, IPackageVersionService packageVersionService, IPackageService packageService, IFileStorageService fileStorageService, ICommandExecuter commandExecuter, ISettingsService settingsService, IInstalledPackagesService installedPackagesService)
        {
            _packageVersionService = packageVersionService;
            _packageService = packageService;
            _availablePackagesService = availablePackagesService;
            _fileStorageService = fileStorageService;
            _commandExecuter = commandExecuter;
            _settingsService = settingsService;
            _installedPackagesService = installedPackagesService;
            _packageVersionService.VersionChanged += VersionChangedHandler;
            _packageVersionService.RunStarted += PackageVersionServiceStarted;
            _availablePackagesService.RunFinshed += PackagesServiceRunFinished;
            _installedPackagesService.RunFinshed += PackagesServiceRunFinished;
            _packageService.RunFinshed += PackageServiceRunFinished;
            _packageService.RunStarted += PackageServiceRunStarted;
			_availablePackagesService.RunFailed += PackagesServiceRunFailed;
            _installedPackagesService.RunFailed += PackagesServiceRunFailed;
            _availablePackagesService.RunStarted += PackagesServiceRunStarted;
            _installedPackagesService.RunStarted += PackagesServiceRunStarted;
            
            InitializeComponent();
            ClearStatus();
            tabAvailable.ImageIndex = 0;
            tabInstalled.ImageIndex = 1;
            _installedPackagesService.ListOfDistinctHighestInstalledPackages();
        }

        private void PackageServiceRunStarted(string message)
        {
            DisableUserInteraction();
            lblprogress.Text = message;
            progressbar1.Visible = true;
            progressbar1.Style = ProgressBarStyle.Marquee;
            tabControlPackage.SelectTab(tabPackageRun);
        }

        private void PackagesServiceRunStarted(string message)
        {
            DisableUserInteraction();
            searchPackages.Text = "";
            lblprogress.Text = message;
            SetStatus();
            progressbar1.Visible = true;
            progressbar1.Style = ProgressBarStyle.Marquee;
        }

        private void PackageVersionServiceStarted(string message)
        {
            DisableUserInteraction();
            lblprogress.Text = message;
            progressbar2.Visible = true;
            progressbar2.Style = ProgressBarStyle.Marquee;
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
                lblprogress.Text = "";
                progressbar2.Visible = false;
                tabControlPackage.SelectTab(tabPackageInformation);
                packageTabControl.SelectTab(tabInstalled);
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

        private void PackagesServiceRunFinished(IList<Package> packages)
        {
            if (InvokeRequired)
            {
                Invoke(new PackageSServiceHandler(PackagesServiceRunFinished), new object[] { packages });
            }
            else
            {
                EnableUserInteraction();
                ClearStatus();
                SetStatus(); 
                Activate();
            }
        }

		private void PackagesServiceRunFailed(Exception exc)
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
                Invoke(new MethodInvoker(() => VersionChangedHandler(version )));
            }
            else
            {
                EnableUserInteraction();
                lblprogress.Text = "";
                progressbar2.Visible = false;
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
            _availablePackagesService.ListOfAvalablePackages();
        }

        private void QueryInstalledPackages()
        {
            var expandedLibDirectory = Environment.ExpandEnvironmentVariables(_settingsService.ChocolateyLibDirectory);
            if (!_fileStorageService.DirectoryExists(expandedLibDirectory))
            {
                MessageBox.Show(string.Format(strings.lib_dir_not_found, expandedLibDirectory));
            }
            else
            {
               _installedPackagesService.ListOfDistinctHighestInstalledPackages();
            }
        }

        private void EnableUserInteraction()
        {
            packageTabControl.Enabled = true;
            mainSplitContainer.Panel1.Enabled = true;
            packageButtonsPanel1.Enabled = true;
            mainMenu.Enabled = true;
        }

        private void DisableUserInteraction()
        {
            packageTabControl.Enabled = true;
            mainSplitContainer.Panel1.Enabled = false;
            packageButtonsPanel1.Enabled = false;
            mainMenu.Enabled = false;
            packageVersionPanel.LockPanel();
        }

        private void SetStatus()
        {
            int rowcount1 = 0;
            int rowcount2 = 0;
            if (availablePackagesGrid1 != null) rowcount1 = availablePackagesGrid1.Rowcount;
            if (installedPackagesGrid1 != null) rowcount2 = installedPackagesGrid1.Rowcount;
            lblStatus.Text = "Available packages: " + rowcount1 + " - Installed packages: " + rowcount2;
        }

        private void ClearStatus()
        {
            lblStatus.Text = "";
            lblprogress.Text = "";
            progressbar1.Visible = false;
        }
    }
}
