using Chocolatey.Explorer.View.Controls;

namespace Chocolatey.Explorer.View.Forms
{
    partial class PackageManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackageManager));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.packagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.availablePackagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installedPackagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.packageTabControl = new System.Windows.Forms.TabControl();
            this.tabInstalled = new System.Windows.Forms.TabPage();
            this.tabAvailable = new System.Windows.Forms.TabPage();
            this.searchBar = new System.Windows.Forms.Panel();
            this.packageTabsImageList = new System.Windows.Forms.ImageList(this.components);
            this.tabControlPackage = new System.Windows.Forms.TabControl();
            this.tabPackageInformation = new System.Windows.Forms.TabPage();
            this.tabPackageRun = new System.Windows.Forms.TabPage();
            this.searchPackages = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewCheckBoxColumn2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewCheckBoxColumn3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewCheckBoxColumn4 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressbar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.progressbar2 = new System.Windows.Forms.ToolStripProgressBar();
            this.lblprogress = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabExtraPackageInformation = new System.Windows.Forms.TabPage();
            this.installedPackagesGrid1 = new Chocolatey.Explorer.View.Controls.InstalledPackagesGrid();
            this.availablePackagesGrid1 = new Chocolatey.Explorer.View.Controls.AvailablePackagesGrid();
            this.packageVersionPanel = new Chocolatey.Explorer.View.Controls.PackageVersionPanel();
            this.packageButtonsPanel1 = new Chocolatey.Explorer.View.Controls.PackageButtonsPanel();
            this.packageRunPanel1 = new Chocolatey.Explorer.View.Controls.PackageRunPanel();
            this.packageExtraInformationPanel1 = new Chocolatey.Explorer.View.Controls.PackageExtraInformationPanel();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            this.packageTabControl.SuspendLayout();
            this.tabInstalled.SuspendLayout();
            this.tabAvailable.SuspendLayout();
            this.tabControlPackage.SuspendLayout();
            this.tabPackageInformation.SuspendLayout();
            this.tabPackageRun.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tabExtraPackageInformation.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            resources.ApplyResources(this.mainMenu, "mainMenu");
            this.mainMenu.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.packagesToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenu.Name = "mainMenu";
            // 
            // packagesToolStripMenuItem
            // 
            resources.ApplyResources(this.packagesToolStripMenuItem, "packagesToolStripMenuItem");
            this.packagesToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuPopup;
            this.packagesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.availablePackagesToolStripMenuItem,
            this.installedPackagesToolStripMenuItem});
            this.packagesToolStripMenuItem.Name = "packagesToolStripMenuItem";
            // 
            // availablePackagesToolStripMenuItem
            // 
            resources.ApplyResources(this.availablePackagesToolStripMenuItem, "availablePackagesToolStripMenuItem");
            this.availablePackagesToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
            this.availablePackagesToolStripMenuItem.Name = "availablePackagesToolStripMenuItem";
            // 
            // installedPackagesToolStripMenuItem
            // 
            resources.ApplyResources(this.installedPackagesToolStripMenuItem, "installedPackagesToolStripMenuItem");
            this.installedPackagesToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
            this.installedPackagesToolStripMenuItem.Image = global::Chocolatey.Explorer.Properties.Resources.monitor_small;
            this.installedPackagesToolStripMenuItem.Name = "installedPackagesToolStripMenuItem";
            // 
            // helpToolStripMenuItem
            // 
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuPopup;
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1,
            this.aboutToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            // 
            // helpToolStripMenuItem1
            // 
            resources.ApplyResources(this.helpToolStripMenuItem1, "helpToolStripMenuItem1");
            this.helpToolStripMenuItem1.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
            this.helpToolStripMenuItem1.Image = global::Chocolatey.Explorer.Properties.Resources.help_small;
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.HelpClick);
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
            this.aboutToolStripMenuItem.Image = global::Chocolatey.Explorer.Properties.Resources.information_small;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutClick);
            // 
            // settingsToolStripMenuItem
            // 
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
            this.settingsToolStripMenuItem.Image = global::Chocolatey.Explorer.Properties.Resources.setting_tools_small;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.SettingsClick);
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.mainSplitContainer, "mainSplitContainer");
            this.mainSplitContainer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.packageTabControl);
            this.mainSplitContainer.Panel1.ForeColor = System.Drawing.Color.Black;
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.tabControlPackage);
            this.mainSplitContainer.TabStop = false;
            // 
            // packageTabControl
            // 
            resources.ApplyResources(this.packageTabControl, "packageTabControl");
            this.packageTabControl.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTabList;
            this.packageTabControl.Controls.Add(this.tabInstalled);
            this.packageTabControl.Controls.Add(this.tabAvailable);
            this.packageTabControl.ImageList = this.packageTabsImageList;
            this.packageTabControl.Name = "packageTabControl";
            this.packageTabControl.SelectedIndex = 0;
            this.packageTabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.packageTabControl_Selected);
            // 
            // tabInstalled
            // 
            resources.ApplyResources(this.tabInstalled, "tabInstalled");
            this.tabInstalled.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.tabInstalled.Controls.Add(this.installedPackagesGrid1);
            this.tabInstalled.Name = "tabInstalled";
            this.tabInstalled.UseVisualStyleBackColor = true;
            // 
            // tabAvailable
            // 
            resources.ApplyResources(this.tabAvailable, "tabAvailable");
            this.tabAvailable.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.tabAvailable.Controls.Add(this.availablePackagesGrid1);
            this.tabAvailable.Controls.Add(this.searchBar);
            this.tabAvailable.Name = "tabAvailable";
            this.tabAvailable.UseVisualStyleBackColor = true;
            // 
            // searchBar
            // 
            resources.ApplyResources(this.searchBar, "searchBar");
            this.searchBar.Name = "searchBar";
            // 
            // packageTabsImageList
            // 
            this.packageTabsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("packageTabsImageList.ImageStream")));
            this.packageTabsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.packageTabsImageList.Images.SetKeyName(0, "chocolateyicon_small.png");
            this.packageTabsImageList.Images.SetKeyName(1, "monitor.png");
            // 
            // tabControlPackage
            // 
            this.tabControlPackage.Controls.Add(this.tabPackageInformation);
            this.tabControlPackage.Controls.Add(this.tabExtraPackageInformation);
            this.tabControlPackage.Controls.Add(this.tabPackageRun);
            resources.ApplyResources(this.tabControlPackage, "tabControlPackage");
            this.tabControlPackage.Name = "tabControlPackage";
            this.tabControlPackage.SelectedIndex = 0;
            // 
            // tabPackageInformation
            // 
            this.tabPackageInformation.BackColor = System.Drawing.SystemColors.Control;
            this.tabPackageInformation.Controls.Add(this.packageVersionPanel);
            this.tabPackageInformation.Controls.Add(this.packageButtonsPanel1);
            resources.ApplyResources(this.tabPackageInformation, "tabPackageInformation");
            this.tabPackageInformation.Name = "tabPackageInformation";
            // 
            // tabPackageRun
            // 
            this.tabPackageRun.BackColor = System.Drawing.SystemColors.Control;
            this.tabPackageRun.Controls.Add(this.packageRunPanel1);
            resources.ApplyResources(this.tabPackageRun, "tabPackageRun");
            this.tabPackageRun.Name = "tabPackageRun";
            // 
            // searchPackages
            // 
            resources.ApplyResources(this.searchPackages, "searchPackages");
            this.searchPackages.Name = "searchPackages";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewCheckBoxColumn2
            // 
            this.dataGridViewCheckBoxColumn2.Name = "dataGridViewCheckBoxColumn2";
            // 
            // dataGridViewCheckBoxColumn3
            // 
            this.dataGridViewCheckBoxColumn3.Name = "dataGridViewCheckBoxColumn3";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // dataGridViewCheckBoxColumn4
            // 
            this.dataGridViewCheckBoxColumn4.Name = "dataGridViewCheckBoxColumn4";
            // 
            // statusStrip
            // 
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.StatusBar;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressbar1,
            this.progressbar2,
            this.lblprogress,
            this.lblStatus});
            this.statusStrip.Name = "statusStrip";
            // 
            // progressbar1
            // 
            resources.ApplyResources(this.progressbar1, "progressbar1");
            this.progressbar1.AccessibleRole = System.Windows.Forms.AccessibleRole.ProgressBar;
            this.progressbar1.Name = "progressbar1";
            this.progressbar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // progressbar2
            // 
            resources.ApplyResources(this.progressbar2, "progressbar2");
            this.progressbar2.AccessibleRole = System.Windows.Forms.AccessibleRole.ProgressBar;
            this.progressbar2.Name = "progressbar2";
            this.progressbar2.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // lblprogress
            // 
            resources.ApplyResources(this.lblprogress, "lblprogress");
            this.lblprogress.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblprogress.Name = "lblprogress";
            this.lblprogress.Spring = true;
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Tag = "test2";
            // 
            // tabExtraPackageInformation
            // 
            this.tabExtraPackageInformation.Controls.Add(this.packageExtraInformationPanel1);
            resources.ApplyResources(this.tabExtraPackageInformation, "tabExtraPackageInformation");
            this.tabExtraPackageInformation.Name = "tabExtraPackageInformation";
            this.tabExtraPackageInformation.UseVisualStyleBackColor = true;
            // 
            // installedPackagesGrid1
            // 
            this.installedPackagesGrid1.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
            resources.ApplyResources(this.installedPackagesGrid1, "installedPackagesGrid1");
            this.installedPackagesGrid1.Name = "installedPackagesGrid1";
            // 
            // availablePackagesGrid1
            // 
            this.availablePackagesGrid1.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
            resources.ApplyResources(this.availablePackagesGrid1, "availablePackagesGrid1");
            this.availablePackagesGrid1.Name = "availablePackagesGrid1";
            // 
            // packageVersionPanel
            // 
            resources.ApplyResources(this.packageVersionPanel, "packageVersionPanel");
            this.packageVersionPanel.Name = "packageVersionPanel";
            // 
            // packageButtonsPanel1
            // 
            resources.ApplyResources(this.packageButtonsPanel1, "packageButtonsPanel1");
            this.packageButtonsPanel1.Name = "packageButtonsPanel1";
            // 
            // packageRunPanel1
            // 
            resources.ApplyResources(this.packageRunPanel1, "packageRunPanel1");
            this.packageRunPanel1.Name = "packageRunPanel1";
            // 
            // packageExtraInformationPanel1
            // 
            this.packageExtraInformationPanel1.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.packageExtraInformationPanel1, "packageExtraInformationPanel1");
            this.packageExtraInformationPanel1.Name = "packageExtraInformationPanel1";
            // 
            // PackageManager
            // 
            resources.ApplyResources(this, "$this");
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Application;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.statusStrip);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "PackageManager";
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.packageTabControl.ResumeLayout(false);
            this.tabInstalled.ResumeLayout(false);
            this.tabAvailable.ResumeLayout(false);
            this.tabControlPackage.ResumeLayout(false);
            this.tabPackageInformation.ResumeLayout(false);
            this.tabPackageRun.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabExtraPackageInformation.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem packagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem availablePackagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installedPackagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar progressbar1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.TabControl packageTabControl;
        private System.Windows.Forms.TabPage tabInstalled;
        private System.Windows.Forms.TabPage tabAvailable;
        private System.Windows.Forms.ImageList packageTabsImageList;
        private PackageVersionPanel packageVersionPanel;
        private System.Windows.Forms.Panel searchBar;
        private System.Windows.Forms.TextBox searchPackages;
        private System.Windows.Forms.Label label1;
        private PackageButtonsPanel packageButtonsPanel1;
        private AvailablePackagesGrid availablePackagesGrid1;
        private InstalledPackagesGrid installedPackagesGrid1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn4;
        private System.Windows.Forms.ToolStripProgressBar progressbar2;
        private System.Windows.Forms.ToolStripStatusLabel lblprogress;
        private System.Windows.Forms.TabControl tabControlPackage;
        private System.Windows.Forms.TabPage tabPackageInformation;
        private System.Windows.Forms.TabPage tabPackageRun;
        private PackageRunPanel packageRunPanel1;
        private System.Windows.Forms.TabPage tabExtraPackageInformation;
        private PackageExtraInformationPanel packageExtraInformationPanel1;
    }
}