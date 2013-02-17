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
            this.searchBar = new System.Windows.Forms.Panel();
            this.searchPackages = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.packageTabControl = new System.Windows.Forms.TabControl();
            this.tabAvailable = new System.Windows.Forms.TabPage();
            this.tabInstalled = new System.Windows.Forms.TabPage();
            this.packageTabsImageList = new System.Windows.Forms.ImageList(this.components);
            this.packageVersionPanel = new Chocolatey.Explorer.View.Controls.PackageVersionPanel();
            this.packageButtonsPanel1 = new Chocolatey.Explorer.View.Controls.PackageButtonsPanel();
            this.txtPowershellOutput = new System.Windows.Forms.TextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblProgressbar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.availablePackagesGrid1 = new Chocolatey.Explorer.View.Controls.AvailablePackagesGrid();
            this.installedPackagesGrid1 = new Chocolatey.Explorer.View.Controls.InstalledPackagesGrid();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            this.searchBar.SuspendLayout();
            this.packageTabControl.SuspendLayout();
            this.tabAvailable.SuspendLayout();
            this.tabInstalled.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.availablePackagesGrid1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.installedPackagesGrid1)).BeginInit();
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
            this.mainSplitContainer.Panel2.Controls.Add(this.packageVersionPanel);
            this.mainSplitContainer.Panel2.Controls.Add(this.packageButtonsPanel1);
            this.mainSplitContainer.Panel2.Controls.Add(this.txtPowershellOutput);
            this.mainSplitContainer.TabStop = false;
            // 
            // searchBar
            // 
            this.searchBar.Controls.Add(this.searchPackages);
            this.searchBar.Controls.Add(this.label1);
            resources.ApplyResources(this.searchBar, "searchBar");
            this.searchBar.Name = "searchBar";
            // 
            // searchPackages
            // 
            resources.ApplyResources(this.searchPackages, "searchPackages");
            this.searchPackages.Name = "searchPackages";
            this.searchPackages.TextChanged += new System.EventHandler(this.searchPackages_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this.label1.Name = "label1";
            // 
            // packageTabControl
            // 
            resources.ApplyResources(this.packageTabControl, "packageTabControl");
            this.packageTabControl.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTabList;
            this.packageTabControl.Controls.Add(this.tabAvailable);
            this.packageTabControl.Controls.Add(this.tabInstalled);
            this.packageTabControl.ImageList = this.packageTabsImageList;
            this.packageTabControl.Name = "packageTabControl";
            this.packageTabControl.SelectedIndex = 0;
            this.packageTabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.packageTabControl_Selected);
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
            // tabInstalled
            // 
            resources.ApplyResources(this.tabInstalled, "tabInstalled");
            this.tabInstalled.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.tabInstalled.Controls.Add(this.installedPackagesGrid1);
            this.tabInstalled.Controls.Add(this.panel1);
            this.tabInstalled.Name = "tabInstalled";
            this.tabInstalled.UseVisualStyleBackColor = true;
            // 
            // packageTabsImageList
            // 
            this.packageTabsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("packageTabsImageList.ImageStream")));
            this.packageTabsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.packageTabsImageList.Images.SetKeyName(0, "chocolateyicon_small.png");
            this.packageTabsImageList.Images.SetKeyName(1, "monitor.png");
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
            // txtPowershellOutput
            // 
            resources.ApplyResources(this.txtPowershellOutput, "txtPowershellOutput");
            this.txtPowershellOutput.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.txtPowershellOutput.Name = "txtPowershellOutput";
            // 
            // statusStrip
            // 
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.StatusBar;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblProgressbar,
            this.lblStatus});
            this.statusStrip.Name = "statusStrip";
            // 
            // lblProgressbar
            // 
            resources.ApplyResources(this.lblProgressbar, "lblProgressbar");
            this.lblProgressbar.AccessibleRole = System.Windows.Forms.AccessibleRole.ProgressBar;
            this.lblProgressbar.Name = "lblProgressbar";
            this.lblProgressbar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Spring = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.label2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this.label2.Name = "label2";
            // 
            // availablePackagesGrid1
            // 
            this.availablePackagesGrid1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.availablePackagesGrid1, "availablePackagesGrid1");
            this.availablePackagesGrid1.Name = "availablePackagesGrid1";
            // 
            // installedPackagesGrid1
            // 
            this.installedPackagesGrid1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.installedPackagesGrid1, "installedPackagesGrid1");
            this.installedPackagesGrid1.Name = "installedPackagesGrid1";
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
            this.mainSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.searchBar.ResumeLayout(false);
            this.searchBar.PerformLayout();
            this.packageTabControl.ResumeLayout(false);
            this.tabAvailable.ResumeLayout(false);
            this.tabInstalled.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.availablePackagesGrid1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.installedPackagesGrid1)).EndInit();
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
        private System.Windows.Forms.TextBox txtPowershellOutput;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar lblProgressbar;
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
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
    }
}