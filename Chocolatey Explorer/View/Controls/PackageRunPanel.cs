using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Chocolatey.Explorer.Extensions;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.PackageService;
using Chocolatey.Explorer.Services.PackageVersionService;
using StructureMap;

namespace Chocolatey.Explorer.View.Controls
{
    public partial class PackageRunPanel : UserControl
    {
        private IPackageVersionService _packageVersionService;
        private IPackageService _packageService;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IPackageService PackageService
        {
            get { return _packageService; }
            set
            {
                _packageService = value;
                _packageService.RunStarted += RunStarted;
                _packageService.LineChanged += LineChanged;
                _packageService.RunFinshed += RunFinished;
            }
        }

        private void RunFinished()
        {
            this.Invoke(() =>
                {
                    txtPowershellOutput.AppendText("Run finished at " + DateTime.Now.ToLocalTime());
                });
        }

        private void LineChanged(string line)
        {
            this.Invoke(() =>
                {
                    txtPowershellOutput.AppendText(line + Environment.NewLine);
                });
        }

        private void RunStarted(string message)
        {
            this.Invoke(() =>
                {
                    txtPowershellOutput.AppendText("Started run at " + DateTime.Now.ToLocalTime() + Environment.NewLine);
                });
        }

        private PackageVersion _version;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IPackageVersionService PackageVersionService
        {
            get
            {
                return _packageVersionService;
            }
            set
            {
                _packageVersionService = value;
                _packageVersionService.VersionChanged += UpdatePanel;
            }
        }

        public PackageRunPanel()
        {
            ObjectFactory.BuildUp(this);

            InitializeComponent();
        }

        /// <summary>
        /// Shows the information of the _version field to
        /// the user.
        /// </summary>
        private void UpdatePanel(PackageVersion version)
        {
            this.Invoke(() =>
                {
                    _version = version;
                    lblName.Text = _version.Name;
                    lblServerVersion.Text = _version.Serverversion;
                    lblInstalledVersion.Text = _version.CurrentVersion;
                    txtPowershellOutput.Text = "";
                    if (!string.IsNullOrEmpty(_version.AuthorName))
                        lblAuthor.Text = string.Format(strings.authored_by, _version.AuthorName);

                    if (_version.IsCurrentVersionPreRelease)
                    {
                        var thisExe = Assembly.GetExecutingAssembly();
                        var file = thisExe.GetManifestResourceStream("Chocolatey.Explorer.Resources.monitorPre.png");
                        if (file != null) picInstalledVersion.Image = Image.FromStream(file);
                    }
                    else
                    {
                        var thisExe = Assembly.GetExecutingAssembly();
                        var file = thisExe.GetManifestResourceStream("Chocolatey.Explorer.Resources.monitor.png");
                        if (file != null) picInstalledVersion.Image = Image.FromStream(file);
                    }
                });
        }
    }
}
