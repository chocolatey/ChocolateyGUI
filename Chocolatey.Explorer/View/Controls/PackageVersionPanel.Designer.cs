namespace Chocolatey.Explorer.View.Controls
{
    partial class PackageVersionPanel
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackageVersionPanel));
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.groupVersionDownloads = new System.Windows.Forms.GroupBox();
            this.lblVersionDownloads = new System.Windows.Forms.Label();
            this.groupTotalDownloads = new System.Windows.Forms.GroupBox();
            this.lblDownloads = new System.Windows.Forms.Label();
            this.groupLastUpdated = new System.Windows.Forms.GroupBox();
            this.lblUpdated = new System.Windows.Forms.Label();
            this.groupPackageSize = new System.Windows.Forms.GroupBox();
            this.lblPackageSize = new System.Windows.Forms.Label();
            this.linkGalleryDetails = new System.Windows.Forms.LinkLabel();
            this.linkProjectSite = new System.Windows.Forms.LinkLabel();
            this.linkLicense = new System.Windows.Forms.LinkLabel();
            this.linkAbuse = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.titleBar = new System.Windows.Forms.TableLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.versionBar = new System.Windows.Forms.TableLayoutPanel();
            this.picServerVersion = new System.Windows.Forms.PictureBox();
            this.lblServerVersion = new System.Windows.Forms.Label();
            this.picInstalledVersion = new System.Windows.Forms.PictureBox();
            this.lblInstalledVersion = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.groupTags = new System.Windows.Forms.GroupBox();
            this.tagList = new System.Windows.Forms.ListView();
            this.groupDependencies = new System.Windows.Forms.GroupBox();
            this.dependenciesList = new System.Windows.Forms.ListView();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.groupVersionDownloads.SuspendLayout();
            this.groupTotalDownloads.SuspendLayout();
            this.groupLastUpdated.SuspendLayout();
            this.groupPackageSize.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.titleBar.SuspendLayout();
            this.versionBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picServerVersion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInstalledVersion)).BeginInit();
            this.groupTags.SuspendLayout();
            this.groupDependencies.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.pictureBoxLogo, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.groupVersionDownloads, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.groupTotalDownloads, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.groupLastUpdated, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.groupPackageSize, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.linkGalleryDetails, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.linkProjectSite, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this.linkLicense, 0, 7);
            this.tableLayoutPanel3.Controls.Add(this.linkAbuse, 0, 8);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // pictureBoxLogo
            // 
            resources.ApplyResources(this.pictureBoxLogo, "pictureBoxLogo");
            this.pictureBoxLogo.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic;
            this.pictureBoxLogo.ErrorImage = global::Chocolatey.Explorer.Properties.Resources.packageicon_default;
            this.pictureBoxLogo.InitialImage = global::Chocolatey.Explorer.Properties.Resources.packageicon_default;
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.TabStop = false;
            // 
            // groupVersionDownloads
            // 
            resources.ApplyResources(this.groupVersionDownloads, "groupVersionDownloads");
            this.groupVersionDownloads.Controls.Add(this.lblVersionDownloads);
            this.groupVersionDownloads.Name = "groupVersionDownloads";
            this.groupVersionDownloads.TabStop = false;
            // 
            // lblVersionDownloads
            // 
            resources.ApplyResources(this.lblVersionDownloads, "lblVersionDownloads");
            this.lblVersionDownloads.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblVersionDownloads.Name = "lblVersionDownloads";
            // 
            // groupTotalDownloads
            // 
            resources.ApplyResources(this.groupTotalDownloads, "groupTotalDownloads");
            this.groupTotalDownloads.Controls.Add(this.lblDownloads);
            this.groupTotalDownloads.Name = "groupTotalDownloads";
            this.groupTotalDownloads.TabStop = false;
            // 
            // lblDownloads
            // 
            resources.ApplyResources(this.lblDownloads, "lblDownloads");
            this.lblDownloads.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblDownloads.Name = "lblDownloads";
            // 
            // groupLastUpdated
            // 
            resources.ApplyResources(this.groupLastUpdated, "groupLastUpdated");
            this.groupLastUpdated.Controls.Add(this.lblUpdated);
            this.groupLastUpdated.Name = "groupLastUpdated";
            this.groupLastUpdated.TabStop = false;
            // 
            // lblUpdated
            // 
            resources.ApplyResources(this.lblUpdated, "lblUpdated");
            this.lblUpdated.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblUpdated.Name = "lblUpdated";
            // 
            // groupPackageSize
            // 
            resources.ApplyResources(this.groupPackageSize, "groupPackageSize");
            this.groupPackageSize.Controls.Add(this.lblPackageSize);
            this.groupPackageSize.Name = "groupPackageSize";
            this.groupPackageSize.TabStop = false;
            // 
            // lblPackageSize
            // 
            resources.ApplyResources(this.lblPackageSize, "lblPackageSize");
            this.lblPackageSize.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblPackageSize.Name = "lblPackageSize";
            // 
            // linkGalleryDetails
            // 
            resources.ApplyResources(this.linkGalleryDetails, "linkGalleryDetails");
            this.linkGalleryDetails.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.linkGalleryDetails.Name = "linkGalleryDetails";
            this.linkGalleryDetails.TabStop = true;
            this.linkGalleryDetails.UseMnemonic = false;
            // 
            // linkProjectSite
            // 
            resources.ApplyResources(this.linkProjectSite, "linkProjectSite");
            this.linkProjectSite.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.linkProjectSite.Name = "linkProjectSite";
            this.linkProjectSite.TabStop = true;
            this.linkProjectSite.UseMnemonic = false;
            // 
            // linkLicense
            // 
            resources.ApplyResources(this.linkLicense, "linkLicense");
            this.linkLicense.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.linkLicense.Name = "linkLicense";
            this.linkLicense.TabStop = true;
            this.linkLicense.UseMnemonic = false;
            // 
            // linkAbuse
            // 
            resources.ApplyResources(this.linkAbuse, "linkAbuse");
            this.linkAbuse.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.linkAbuse.Name = "linkAbuse";
            this.linkAbuse.TabStop = true;
            this.linkAbuse.UseMnemonic = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.titleBar, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.versionBar, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtDescription, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupTags, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.groupDependencies, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // titleBar
            // 
            resources.ApplyResources(this.titleBar, "titleBar");
            this.titleBar.Controls.Add(this.lblName, 0, 0);
            this.titleBar.Controls.Add(this.lblAuthor, 1, 0);
            this.titleBar.Name = "titleBar";
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblName.Name = "lblName";
            // 
            // lblAuthor
            // 
            resources.ApplyResources(this.lblAuthor, "lblAuthor");
            this.lblAuthor.Name = "lblAuthor";
            // 
            // versionBar
            // 
            resources.ApplyResources(this.versionBar, "versionBar");
            this.versionBar.Controls.Add(this.picServerVersion, 0, 0);
            this.versionBar.Controls.Add(this.lblServerVersion, 1, 0);
            this.versionBar.Controls.Add(this.picInstalledVersion, 2, 0);
            this.versionBar.Controls.Add(this.lblInstalledVersion, 3, 0);
            this.versionBar.Name = "versionBar";
            // 
            // picServerVersion
            // 
            resources.ApplyResources(this.picServerVersion, "picServerVersion");
            this.picServerVersion.Name = "picServerVersion";
            this.picServerVersion.TabStop = false;
            // 
            // lblServerVersion
            // 
            resources.ApplyResources(this.lblServerVersion, "lblServerVersion");
            this.lblServerVersion.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblServerVersion.Name = "lblServerVersion";
            // 
            // picInstalledVersion
            // 
            this.picInstalledVersion.Image = global::Chocolatey.Explorer.Properties.Resources.monitor;
            resources.ApplyResources(this.picInstalledVersion, "picInstalledVersion");
            this.picInstalledVersion.Name = "picInstalledVersion";
            this.picInstalledVersion.TabStop = false;
            // 
            // lblInstalledVersion
            // 
            resources.ApplyResources(this.lblInstalledVersion, "lblInstalledVersion");
            this.lblInstalledVersion.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblInstalledVersion.Name = "lblInstalledVersion";
            // 
            // txtDescription
            // 
            this.txtDescription.AcceptsReturn = true;
            resources.ApplyResources(this.txtDescription, "txtDescription");
            this.txtDescription.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.txtDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            // 
            // groupTags
            // 
            resources.ApplyResources(this.groupTags, "groupTags");
            this.groupTags.Controls.Add(this.tagList);
            this.groupTags.Name = "groupTags";
            this.groupTags.TabStop = false;
            // 
            // tagList
            // 
            resources.ApplyResources(this.tagList, "tagList");
            this.tagList.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
            this.tagList.BackColor = System.Drawing.SystemColors.Control;
            this.tagList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tagList.GridLines = true;
            this.tagList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.tagList.MultiSelect = false;
            this.tagList.Name = "tagList";
            this.tagList.ShowGroups = false;
            this.tagList.TileSize = new System.Drawing.Size(40, 20);
            this.tagList.UseCompatibleStateImageBehavior = false;
            this.tagList.View = System.Windows.Forms.View.SmallIcon;
            // 
            // groupDependencies
            // 
            resources.ApplyResources(this.groupDependencies, "groupDependencies");
            this.groupDependencies.Controls.Add(this.dependenciesList);
            this.groupDependencies.Name = "groupDependencies";
            this.groupDependencies.TabStop = false;
            // 
            // dependenciesList
            // 
            resources.ApplyResources(this.dependenciesList, "dependenciesList");
            this.dependenciesList.BackColor = System.Drawing.SystemColors.Control;
            this.dependenciesList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dependenciesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.dependenciesList.MultiSelect = false;
            this.dependenciesList.Name = "dependenciesList";
            this.dependenciesList.ShowGroups = false;
            this.dependenciesList.UseCompatibleStateImageBehavior = false;
            this.dependenciesList.View = System.Windows.Forms.View.SmallIcon;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // PackageVersionPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "PackageVersionPanel";
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.groupVersionDownloads.ResumeLayout(false);
            this.groupVersionDownloads.PerformLayout();
            this.groupTotalDownloads.ResumeLayout(false);
            this.groupTotalDownloads.PerformLayout();
            this.groupLastUpdated.ResumeLayout(false);
            this.groupLastUpdated.PerformLayout();
            this.groupPackageSize.ResumeLayout(false);
            this.groupPackageSize.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.titleBar.ResumeLayout(false);
            this.titleBar.PerformLayout();
            this.versionBar.ResumeLayout(false);
            this.versionBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picServerVersion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInstalledVersion)).EndInit();
            this.groupTags.ResumeLayout(false);
            this.groupDependencies.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.GroupBox groupTotalDownloads;
        private System.Windows.Forms.Label lblDownloads;
        private System.Windows.Forms.GroupBox groupVersionDownloads;
        private System.Windows.Forms.Label lblVersionDownloads;
        private System.Windows.Forms.GroupBox groupLastUpdated;
        private System.Windows.Forms.Label lblUpdated;
        private System.Windows.Forms.GroupBox groupPackageSize;
        private System.Windows.Forms.Label lblPackageSize;
        private System.Windows.Forms.LinkLabel linkGalleryDetails;
        private System.Windows.Forms.LinkLabel linkLicense;
        private System.Windows.Forms.LinkLabel linkProjectSite;
        private System.Windows.Forms.LinkLabel linkAbuse;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel versionBar;
        private System.Windows.Forms.Label lblServerVersion;
        private System.Windows.Forms.PictureBox picServerVersion;
        private System.Windows.Forms.Label lblInstalledVersion;
        private System.Windows.Forms.PictureBox picInstalledVersion;
        private System.Windows.Forms.TableLayoutPanel titleBar;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.GroupBox groupTags;
        private System.Windows.Forms.ListView tagList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox groupDependencies;
        private System.Windows.Forms.ListView dependenciesList;
        private System.Windows.Forms.TextBox txtDescription;



    }
}
