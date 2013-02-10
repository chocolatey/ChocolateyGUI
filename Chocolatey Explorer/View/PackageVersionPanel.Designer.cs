namespace Chocolatey.Explorer.View
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblDownloads = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblVersionDownloads = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblUpdated = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblPackageSize = new System.Windows.Forms.Label();
            this.linkGalleryDetails = new System.Windows.Forms.LinkLabel();
            this.linkLicense = new System.Windows.Forms.LinkLabel();
            this.linkProjectSite = new System.Windows.Forms.LinkLabel();
            this.linkAbuse = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.dependenciesList = new System.Windows.Forms.ListView();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.lblServerVersion = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblInstalledVersion = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tagList = new System.Windows.Forms.ListView();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.pictureBoxLogo, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.groupBox3, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.groupBox4, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.linkGalleryDetails, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.linkLicense, 0, 7);
            this.tableLayoutPanel3.Controls.Add(this.linkProjectSite, 0, 6);
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
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.lblDownloads);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lblDownloads
            // 
            resources.ApplyResources(this.lblDownloads, "lblDownloads");
            this.lblDownloads.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblDownloads.Name = "lblDownloads";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.lblVersionDownloads);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // lblVersionDownloads
            // 
            resources.ApplyResources(this.lblVersionDownloads, "lblVersionDownloads");
            this.lblVersionDownloads.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblVersionDownloads.Name = "lblVersionDownloads";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.lblUpdated);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // lblUpdated
            // 
            resources.ApplyResources(this.lblUpdated, "lblUpdated");
            this.lblUpdated.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblUpdated.Name = "lblUpdated";
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.lblPackageSize);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
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
            // linkLicense
            // 
            resources.ApplyResources(this.linkLicense, "linkLicense");
            this.linkLicense.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.linkLicense.Name = "linkLicense";
            this.linkLicense.TabStop = true;
            this.linkLicense.UseMnemonic = false;
            // 
            // linkProjectSite
            // 
            resources.ApplyResources(this.linkProjectSite, "linkProjectSite");
            this.linkProjectSite.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.linkProjectSite.Name = "linkProjectSite";
            this.linkProjectSite.TabStop = true;
            this.linkProjectSite.UseMnemonic = false;
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
            this.tableLayoutPanel1.Controls.Add(this.groupBox6, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtDescription, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox5, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.dependenciesList);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
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
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.lblServerVersion, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.lblInstalledVersion, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.pictureBox2, 2, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // lblServerVersion
            // 
            resources.ApplyResources(this.lblServerVersion, "lblServerVersion");
            this.lblServerVersion.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblServerVersion.Name = "lblServerVersion";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // lblInstalledVersion
            // 
            resources.ApplyResources(this.lblInstalledVersion, "lblInstalledVersion");
            this.lblInstalledVersion.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblInstalledVersion.Name = "lblInstalledVersion";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Chocolatey.Explorer.Properties.Resources.monitor;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
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
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.lblName, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.lblAuthor, 1, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
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
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.tagList);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // tagList
            // 
            resources.ApplyResources(this.tagList, "tagList");
            this.tagList.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
            this.tagList.BackColor = System.Drawing.SystemColors.Control;
            this.tagList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tagList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.tagList.MultiSelect = false;
            this.tagList.Name = "tagList";
            this.tagList.ShowGroups = false;
            this.tagList.UseCompatibleStateImageBehavior = false;
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblDownloads;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblVersionDownloads;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblUpdated;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lblPackageSize;
        private System.Windows.Forms.LinkLabel linkGalleryDetails;
        private System.Windows.Forms.LinkLabel linkLicense;
        private System.Windows.Forms.LinkLabel linkProjectSite;
        private System.Windows.Forms.LinkLabel linkAbuse;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label lblServerVersion;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblInstalledVersion;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ListView tagList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ListView dependenciesList;



    }
}
