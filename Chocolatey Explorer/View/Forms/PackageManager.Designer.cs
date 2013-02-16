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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.packagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.availablePackagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installedPackagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.PackageGrid = new System.Windows.Forms.DataGridView();
            this.searchBar = new System.Windows.Forms.Panel();
            this.searchPackages = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.packageTabControl = new System.Windows.Forms.TabControl();
            this.tabAvailable = new System.Windows.Forms.TabPage();
            this.tabInstalled = new System.Windows.Forms.TabPage();
            this.packageTabsImageList = new System.Windows.Forms.ImageList(this.components);
            this.packageVersionPanel = new Chocolatey.Explorer.View.Controls.PackageVersionPanel();
            this.buttonRow = new System.Windows.Forms.TableLayoutPanel();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnInstallUninstall = new System.Windows.Forms.CheckBox();
            this.installUninstallImageList = new System.Windows.Forms.ImageList(this.components);
            this.txtPowershellOutput = new System.Windows.Forms.TextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblProgressbar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.IsInstalled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InstalledVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Prerelease = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PackageGrid)).BeginInit();
            this.searchBar.SuspendLayout();
            this.packageTabControl.SuspendLayout();
            this.buttonRow.SuspendLayout();
            this.statusStrip.SuspendLayout();
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
            this.availablePackagesToolStripMenuItem.Click += new System.EventHandler(this.availablePackages_Click);
            // 
            // installedPackagesToolStripMenuItem
            // 
            resources.ApplyResources(this.installedPackagesToolStripMenuItem, "installedPackagesToolStripMenuItem");
            this.installedPackagesToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
            this.installedPackagesToolStripMenuItem.Image = global::Chocolatey.Explorer.Properties.Resources.monitor_small;
            this.installedPackagesToolStripMenuItem.Name = "installedPackagesToolStripMenuItem";
            this.installedPackagesToolStripMenuItem.Click += new System.EventHandler(this.installedPackages_Click);
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
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.help_Click);
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
            this.aboutToolStripMenuItem.Image = global::Chocolatey.Explorer.Properties.Resources.information_small;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.about_Click);
            // 
            // settingsToolStripMenuItem
            // 
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
            this.settingsToolStripMenuItem.Image = global::Chocolatey.Explorer.Properties.Resources.setting_tools_small;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settings_Click);
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
            this.mainSplitContainer.Panel1.Controls.Add(this.PackageGrid);
            this.mainSplitContainer.Panel1.Controls.Add(this.searchBar);
            this.mainSplitContainer.Panel1.Controls.Add(this.packageTabControl);
            this.mainSplitContainer.Panel1.ForeColor = System.Drawing.Color.Black;
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.packageVersionPanel);
            this.mainSplitContainer.Panel2.Controls.Add(this.buttonRow);
            this.mainSplitContainer.Panel2.Controls.Add(this.txtPowershellOutput);
            this.mainSplitContainer.TabStop = false;
            // 
            // PackageGrid
            // 
            resources.ApplyResources(this.PackageGrid, "PackageGrid");
            this.PackageGrid.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
            this.PackageGrid.AllowUserToAddRows = false;
            this.PackageGrid.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            this.PackageGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.PackageGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            this.PackageGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PackageGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.PackageGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.PackageGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PackageGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IsInstalled,
            this.Column1,
            this.InstalledVersion,
            this.Prerelease});
            this.PackageGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.PackageGrid.GridColor = System.Drawing.SystemColors.Control;
            this.PackageGrid.MultiSelect = false;
            this.PackageGrid.Name = "PackageGrid";
            this.PackageGrid.ReadOnly = true;
            this.PackageGrid.RowHeadersVisible = false;
            this.PackageGrid.RowTemplate.ReadOnly = true;
            this.PackageGrid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PackageGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.PackageGrid.ShowCellErrors = false;
            this.PackageGrid.ShowCellToolTips = false;
            this.PackageGrid.ShowEditingIcon = false;
            this.PackageGrid.ShowRowErrors = false;
            this.PackageGrid.StandardTab = true;
            this.PackageGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.PackageGrid_CellContentClick);
            this.PackageGrid.SelectionChanged += new System.EventHandler(this.PackageGrid_SelectionChanged);
            this.PackageGrid.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PackageGrid_KeyPress);
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
            this.tabAvailable.Name = "tabAvailable";
            this.tabAvailable.UseVisualStyleBackColor = true;
            // 
            // tabInstalled
            // 
            resources.ApplyResources(this.tabInstalled, "tabInstalled");
            this.tabInstalled.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
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
            // buttonRow
            // 
            resources.ApplyResources(this.buttonRow, "buttonRow");
            this.buttonRow.Controls.Add(this.btnUpdate, 0, 0);
            this.buttonRow.Controls.Add(this.btnInstallUninstall, 1, 0);
            this.buttonRow.Name = "buttonRow";
            // 
            // btnUpdate
            // 
            resources.ApplyResources(this.btnUpdate, "btnUpdate");
            this.btnUpdate.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnUpdate.Image = global::Chocolatey.Explorer.Properties.Resources.update;
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // btnInstallUninstall
            // 
            resources.ApplyResources(this.btnInstallUninstall, "btnInstallUninstall");
            this.btnInstallUninstall.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnInstallUninstall.AutoCheck = false;
            this.btnInstallUninstall.ImageList = this.installUninstallImageList;
            this.btnInstallUninstall.Name = "btnInstallUninstall";
            this.btnInstallUninstall.UseVisualStyleBackColor = true;
            this.btnInstallUninstall.CheckStateChanged += new System.EventHandler(this.btnInstallUninstall_CheckStateChanged);
            this.btnInstallUninstall.Click += new System.EventHandler(this.buttonInstallUninstall_Click);
            // 
            // installUninstallImageList
            // 
            this.installUninstallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("installUninstallImageList.ImageStream")));
            this.installUninstallImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.installUninstallImageList.Images.SetKeyName(0, "add.png");
            this.installUninstallImageList.Images.SetKeyName(1, "delete.png");
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
            // IsInstalled
            // 
            this.IsInstalled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.IsInstalled.DataPropertyName = "IsInstalled";
            resources.ApplyResources(this.IsInstalled, "IsInstalled");
            this.IsInstalled.Name = "IsInstalled";
            this.IsInstalled.ReadOnly = true;
            this.IsInstalled.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.DataPropertyName = "Name";
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Column1.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.Column1, "Column1");
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // InstalledVersion
            // 
            this.InstalledVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.InstalledVersion.DataPropertyName = "InstalledVersion";
            dataGridViewCellStyle4.NullValue = "no version";
            this.InstalledVersion.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.InstalledVersion, "InstalledVersion");
            this.InstalledVersion.Name = "InstalledVersion";
            this.InstalledVersion.ReadOnly = true;
            this.InstalledVersion.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.InstalledVersion.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Prerelease
            // 
            this.Prerelease.DataPropertyName = "IsPreRelease";
            resources.ApplyResources(this.Prerelease, "Prerelease");
            this.Prerelease.Name = "Prerelease";
            this.Prerelease.ReadOnly = true;
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
            ((System.ComponentModel.ISupportInitialize)(this.PackageGrid)).EndInit();
            this.searchBar.ResumeLayout(false);
            this.searchBar.PerformLayout();
            this.packageTabControl.ResumeLayout(false);
            this.buttonRow.ResumeLayout(false);
            this.buttonRow.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
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
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.TextBox txtPowershellOutput;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar lblProgressbar;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.TableLayoutPanel buttonRow;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.TabControl packageTabControl;
        private System.Windows.Forms.TabPage tabInstalled;
        private System.Windows.Forms.TabPage tabAvailable;
        private System.Windows.Forms.CheckBox btnInstallUninstall;
        private System.Windows.Forms.ImageList installUninstallImageList;
        private System.Windows.Forms.ImageList packageTabsImageList;
        private PackageVersionPanel packageVersionPanel;
        private System.Windows.Forms.DataGridView PackageGrid;
        private System.Windows.Forms.Panel searchBar;
        private System.Windows.Forms.TextBox searchPackages;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsInstalled;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn InstalledVersion;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Prerelease;
    }
}