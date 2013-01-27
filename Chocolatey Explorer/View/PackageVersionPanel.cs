using System.ComponentModel;
using System.Windows.Forms;
using Chocolatey.Explorer.Model;
using System;

namespace Chocolatey.Explorer.View
{
    /// <summary>
    /// Shows detailed information of any PackageVersion instance.
    /// Just set the Version field to the package you want to show
    /// to the user.
    /// </summary>
    public partial class PackageVersionPanel : UserControl, IComponent
    {
        public PackageVersion Version {
            set 
            { 
                _version = value;
                UpdatePanel();
            } 
        }

        private PackageVersion _version;

        public PackageVersionPanel()
        {
            InitializeComponent();

            linkGalleryDetails.LinkClicked += OnGalleryDetailsClicked;
            linkLicense.LinkClicked += OnLiscenseClicked;
            linkAbuse.LinkClicked += OnProjectSiteClicked;
            linkAbuse.LinkClicked += OnAbuseClicked;
        }

        /// <summary>
        /// Shows the information of the _version field to
        /// the user.
        /// </summary>
        private void UpdatePanel()
        {
            lblName.Text = _version.Name;
            lblServerVersion.Text = _version.Serverversion;
            lblInstalledVersion.Text = _version.CurrentVersion;

            if (_version.AuthorName != null && _version.AuthorName != "")
                lblAuthor.Text = "by " + _version.AuthorName;

            txtDescription.Text = _version.Description;

            if (_version.IconUrl != null && _version.IconUrl != "")
            {
                pictureBoxLogo.ImageLocation = _version.IconUrl;
                pictureBoxLogo.LoadAsync();
            }

            if (_version.DownloadCount != 0)
                lblDownloads.Text = _version.DownloadCount.ToString();
            if (_version.VersionDownloadCount != 0)
                lblVersionDownloads.Text = _version.VersionDownloadCount.ToString();
            if (_version.LastUpdatedAt != DateTime.MinValue)
                lblUpdated.Text = _version.LastUpdatedAt.GetDateTimeFormats()[0];
            if (_version.PackageSize != 0)
                lblPackageSize.Text = String.Format("{0:0.#}", (_version.PackageSize / 1024.0)) + " KB";

            linkGalleryDetails.Enabled = (_version.GalleryDetailsUrl != "" && _version.GalleryDetailsUrl != null);
            linkLicense.Enabled = (_version.LicenseUrl != "" && _version.LicenseUrl != null);
            linkProjectSite.Enabled = (_version.ProjectUrl != "" && _version.ProjectUrl != null);
            linkAbuse.Enabled = (_version.ReportAbuseUrl != "" && _version.ReportAbuseUrl != null);

            if (_version.Tags != null)
            {
                foreach (var tag in _version.Tags)
                    tagList.Items.Add("#" + tag);
            } 
            if (_version.Dependencies != null)
            {
                foreach (var dependency in _version.Dependencies)
                    dependenciesList.Items.Add(dependency);
            }
        }

        /// <summary>
        /// Resets all Textfields and user controls.
        /// </summary>
        public void ClearPanel()
        {
            linkGalleryDetails.LinkVisited = false;
            linkLicense.LinkVisited = false;
            linkProjectSite.LinkVisited = false;
            linkAbuse.LinkVisited = false;
            linkGalleryDetails.Enabled = false;
            linkLicense.Enabled = false;
            linkProjectSite.Enabled = false;
            linkAbuse.Enabled = false;
            lblName.Text = "";
            lblServerVersion.Text = "";
            lblInstalledVersion.Text = "";
            lblAuthor.Text = "";
            txtDescription.Text = ""; 
            pictureBoxLogo.Image = pictureBoxLogo.ErrorImage;
            lblDownloads.Text = "n.a.";
            lblVersionDownloads.Text = "n.a.";
            lblUpdated.Text = "n.a.";
            lblPackageSize.Text = "n.a.";
            tagList.Items.Clear();
            dependenciesList.Items.Clear();
        }

        private void OnGalleryDetailsClicked(object sender, LinkLabelLinkClickedEventArgs args)
        {
            linkGalleryDetails.LinkVisited = true;
            System.Diagnostics.Process.Start(_version.GalleryDetailsUrl);
        }

        private void OnLiscenseClicked(object sender, LinkLabelLinkClickedEventArgs args)
        {
            linkLicense.LinkVisited = true;
            System.Diagnostics.Process.Start(_version.LicenseUrl);
        }

        private void OnProjectSiteClicked(object sender, LinkLabelLinkClickedEventArgs args)
        {
            linkProjectSite.LinkVisited = true;
            System.Diagnostics.Process.Start(_version.ProjectUrl);
        }

        private void OnAbuseClicked(object sender, LinkLabelLinkClickedEventArgs args)
        {
            linkAbuse.LinkVisited = true;
            System.Diagnostics.Process.Start(_version.ReportAbuseUrl);
        }
    }
}
