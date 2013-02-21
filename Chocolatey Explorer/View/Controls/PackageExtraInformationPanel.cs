using System;
using System.ComponentModel;
using System.Windows.Forms;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.PackageVersionService;
using StructureMap;

namespace Chocolatey.Explorer.View.Controls
{
    public partial class PackageExtraInformationPanel : UserControl
    {
        private IPackageVersionService _packageVersionService;
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

        public PackageExtraInformationPanel()
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
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => UpdatePanel(version)));
            }
            else
            {
                _version = version;
                txtCopyrightInformation.Text = _version.CopyrightInformation;
                txtReleaseNotes.Text = _version.ReleaseNotes != null ? _version.ReleaseNotes.Replace("\n\t\n\t", Environment.NewLine) : "";
            }
        }
    }
}
