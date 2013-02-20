using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.PackageVersionService;
using StructureMap;

namespace Chocolatey.Explorer.View.Controls
{

    public partial class PackageVersionPanel : UserControl
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

        public PackageVersionPanel()
        {
            ObjectFactory.BuildUp(this);

            InitializeComponent();
            ClearPanel();

            linkGalleryDetails.LinkClicked += OnGalleryDetailsClicked;
            linkLicense.LinkClicked += OnLiscenseClicked;
            linkProjectSite.LinkClicked += OnProjectSiteClicked;
            linkAbuse.LinkClicked += OnAbuseClicked;
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
                ClearPanel();
                lblName.Text = _version.Name;
                lblServerVersion.Text = _version.Serverversion;
                lblInstalledVersion.Text = _version.CurrentVersion;

                if (!string.IsNullOrEmpty(_version.AuthorName))
                    lblAuthor.Text = string.Format(strings.authored_by, _version.AuthorName);

                txtDescription.Text = _version.Description;

                if (!string.IsNullOrEmpty(_version.IconUrl))
                {
                    pictureBoxLogo.ImageLocation = _version.IconUrl;
                    pictureBoxLogo.LoadAsync();
                }
                else
                {
                    pictureBoxLogo.Image = pictureBoxLogo.ErrorImage;
                }
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

                if (_version.DownloadCount != 0)
                    lblDownloads.Text = _version.DownloadCount.ToString(CultureInfo.InvariantCulture);
                if (_version.VersionDownloadCount != 0)
                    lblVersionDownloads.Text = _version.VersionDownloadCount.ToString(CultureInfo.InvariantCulture);
                if (_version.LastUpdatedAt != DateTime.MinValue)
                    lblUpdated.Text = _version.LastUpdatedAt.GetDateTimeFormats()[0];
                if (_version.PackageSize != 0)
                    lblPackageSize.Text = string.Format(strings.package_size_mb, (_version.PackageSize / 1024.0));

                tagList.Items.Clear();
                if (_version.Tags != null)
                {
                    foreach (var tag in _version.Tags)
                        tagList.Items.Add("#" + tag);
                }
                dependenciesList.Items.Clear();
                if (_version.Dependencies != null)
                {
                    foreach (var dependency in _version.Dependencies)
                        dependenciesList.Items.Add(dependency);
                }

                UnlockPanel();
            }
        }

        /// <summary>
        /// Locks all user controls.
        /// </summary>
        public void LockPanel()
        {
            linkGalleryDetails.LinkVisited = false;
            linkLicense.LinkVisited = false;
            linkProjectSite.LinkVisited = false;
            linkAbuse.LinkVisited = false;
            tagList.Enabled = false;
            dependenciesList.Enabled = false;
        }

        /// <summary>
        /// Locks all user controls.
        /// </summary>
        private void UnlockPanel()
        {
            linkGalleryDetails.Enabled = !string.IsNullOrEmpty(_version.GalleryDetailsUrl);
            linkLicense.Enabled = !string.IsNullOrEmpty(_version.LicenseUrl);
            linkProjectSite.Enabled = !string.IsNullOrEmpty(_version.ProjectUrl);
            linkAbuse.Enabled = !string.IsNullOrEmpty(_version.ReportAbuseUrl);
            tagList.Enabled = true;
            dependenciesList.Enabled = true;
        }

        /// <summary>
        /// Resets all Textfields and user controls.
        /// </summary>
        private void ClearPanel()
        {
            lblName.Text = "";
            lblServerVersion.Text = "";
            lblInstalledVersion.Text = "";
            lblAuthor.Text = "";
            txtDescription.Text = "";
            pictureBoxLogo.Image = pictureBoxLogo.ErrorImage;
            lblDownloads.Text = strings.not_available;
            lblVersionDownloads.Text = strings.not_available;
            lblUpdated.Text = strings.not_available;
            lblPackageSize.Text = strings.not_available;
            tagList.Items.Clear();
            dependenciesList.Items.Clear();
        }

        private void OnGalleryDetailsClicked(object sender, LinkLabelLinkClickedEventArgs args)
        {
            System.Diagnostics.Process.Start(_version.GalleryDetailsUrl);
        }

        private void OnLiscenseClicked(object sender, LinkLabelLinkClickedEventArgs args)
        {
            System.Diagnostics.Process.Start(_version.LicenseUrl);
        }

        private void OnProjectSiteClicked(object sender, LinkLabelLinkClickedEventArgs args)
        {
            System.Diagnostics.Process.Start(_version.ProjectUrl);
        }

        private void OnAbuseClicked(object sender, LinkLabelLinkClickedEventArgs args)
        {
            System.Diagnostics.Process.Start(_version.ReportAbuseUrl);
        }

    }
}
