using System;
using System.ComponentModel;
using System.Windows.Forms;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.PackageService;
using Chocolatey.Explorer.Services.PackageVersionService;
using StructureMap;

namespace Chocolatey.Explorer.View.Controls
{
    partial class PackageButtonsPanel : UserControl
    {
        private IPackageVersionService _packageVersionService;
        private IPackageService _packageService;
        private PackageVersion _version;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IPackageService PackageService
        {
            get { return _packageService; }
            set { _packageService = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IPackageVersionService PackageVersionService
        {
            get
            {
                return _packageVersionService;
            }
            set
            {
                _packageVersionService = value;
                _packageVersionService.VersionChanged += UpdateInstallUninstallButtonLabel;
            }
        }

        public PackageButtonsPanel()
        {
            ObjectFactory.BuildUp(this);

            InitializeComponent();

            btnUpdate.Click += BtnUpdateClick;
            btnInstallUninstall.Click += BtnInstallUninstallClick;
        }

        private void UpdateInstallUninstallButtonLabel(PackageVersion version)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => UpdateInstallUninstallButtonLabel(version)));
            }
            else
            {
                _version = version;
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
        }
        
        private void BtnUpdateClick(object sender, EventArgs e)
        {
            btnUpdate.Enabled = _version.CanBeUpdated;
            btnInstallUninstall.Checked = !_version.IsInstalled;
            btnInstallUninstall.Enabled = true;
            _packageService.UpdatePackage(_version.Name);
        }

        private void BtnInstallUninstallClick(object sender, EventArgs e)
        {
            if (_version.IsInstalled)
            {
                var result = MessageBox.Show(this, 
                    string.Format(strings.really_uninstall_package_msg, _version.Name),
                    strings.really_uninstall_package_title,
                    MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    _packageService.UninstallPackage(_version.Name);
                }
            }
            else
            {
                _packageService.InstallPackage(_version.Name);
            }
        }
    }
}