using System;
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
        }

        private void PackageServiceRunFinished()
        {
            if (this.InvokeRequired)
            {
                Invoke(new PackageServiceRunFinishedHandler(PackageServiceRunFinished));
            }
            else
            {
                ClearStatus();
                txtPowershellOutput.Visible = false;
                SelectPackage();
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
                var distinctpackages = packages.Distinct().ToList();
                PackageList.DataSource = distinctpackages;
                PackageList.DisplayMember = "Name";
                if(distinctpackages.Count > 0) PackageList.SelectedIndex = 0;
                ClearStatus();
                lblStatus.Text = "Number of installed packages: " + packages.Count;
                SelectPackage(); 
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
                txtVersion.Text = "";
                txtVersion.AppendText(version.Name + Environment.NewLine);
                txtVersion.Select(0, version.Name.Length);
                txtVersion.SelectionFont = new Font(txtVersion.SelectionFont.FontFamily, 12, FontStyle.Bold);
                txtVersion.AppendText("Current version: " + version.CurrentVersion + Environment.NewLine);
                txtVersion.AppendText("Version on the server: " + version.Serverversion + Environment.NewLine);
                btnUpdate.Enabled = version.CanBeUpdated;
                btnInstall.Enabled = !version.IsInstalled;
                ClearStatus();
                lblStatus.Text = "Number of packages: " + PackageList.Items.Count;
                PackageList.Enabled = true;
            }
        }

        public PackageManager(PackagesService packagesService)
        {
            _packagesService = packagesService;
            ClearStatus();
        }

        private void availablePackagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStatus("Getting list of packages on server");
            lblPackages.Text = "Available packages";
            lblProgressbar.Style = ProgressBarStyle.Marquee;
            _packagesService.ListOfPackages();
        }

        private void installedPackagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settings = new Properties.Settings();
            var expandedLibDirectory = System.Environment.ExpandEnvironmentVariables(settings.ChocolateyLibDirectory);
            if (!System.IO.Directory.Exists(expandedLibDirectory))
            {
                MessageBox.Show("Could not find the installed packages directory (" + expandedLibDirectory + "), please change the install directory in the settings.");
            }
            else
            {
                SetStatus("Getting list of installed packages");
                lblPackages.Text = "Installed packages";
                lblProgressbar.Style = ProgressBarStyle.Marquee;
                _packagesService.ListOfInstalledPackages();    
            }
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var help = new Help();
            help.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            SelectPackage();
        }

        private void SelectPackage()
        {
            PackageList.Enabled = false;
            if (PackageList.SelectedItem == null) return;
            SetStatus("Getting package information for package: " + ((Package)PackageList.SelectedItem).Name);
            EmptyTextBoxes();
            _packageVersionService.PackageVersion(((Package) PackageList.SelectedItem).Name);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (PackageList.SelectedItem == null) return;
            SetStatus("Updating package " + ((Package)PackageList.SelectedItem).Name);
            txtPowershellOutput.Visible = true;
            _packageService.UpdatePackage(((Package) PackageList.SelectedItem).Name);
        }

        private void EmptyTextBoxes()
        {
            txtVersion.Text = "";
            txtPowershellOutput.Text = "";
            btnUpdate.Enabled = false;
            btnInstall.Enabled = false;
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (PackageList.SelectedItem == null) return;
            SetStatus("Installing package " + ((Package) PackageList.SelectedItem).Name);
            txtPowershellOutput.Visible = true;
            _packageService.InstallPackage(((Package) PackageList.SelectedItem).Name);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settings = new Settings();
            settings.ShowDialog();
        }
    }
}
