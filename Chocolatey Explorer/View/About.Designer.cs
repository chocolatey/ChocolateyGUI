namespace Chocolatey.Explorer.View
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.latestVersionBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabelChocolatey = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabelExplorer = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.linkIcons = new System.Windows.Forms.LinkLabel();
            this.labelIcons = new System.Windows.Forms.Label();
            this.linkLabelCC = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // latestVersionBox
            // 
            resources.ApplyResources(this.latestVersionBox, "latestVersionBox");
            this.latestVersionBox.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.latestVersionBox.Name = "latestVersionBox";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // linkLabelChocolatey
            // 
            resources.ApplyResources(this.linkLabelChocolatey, "linkLabelChocolatey");
            this.linkLabelChocolatey.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.linkLabelChocolatey.Name = "linkLabelChocolatey";
            this.linkLabelChocolatey.TabStop = true;
            this.linkLabelChocolatey.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelChocolatey_LinkClicked);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // linkLabelExplorer
            // 
            resources.ApplyResources(this.linkLabelExplorer, "linkLabelExplorer");
            this.linkLabelExplorer.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.linkLabelExplorer.Name = "linkLabelExplorer";
            this.linkLabelExplorer.TabStop = true;
            this.linkLabelExplorer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelExplorer_LinkClicked);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // lblVersion
            // 
            resources.ApplyResources(this.lblVersion, "lblVersion");
            this.lblVersion.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblVersion.Name = "lblVersion";
            // 
            // progressBar
            // 
            resources.ApplyResources(this.progressBar, "progressBar");
            this.progressBar.AccessibleRole = System.Windows.Forms.AccessibleRole.ProgressBar;
            this.progressBar.Name = "progressBar";
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // linkIcons
            // 
            resources.ApplyResources(this.linkIcons, "linkIcons");
            this.linkIcons.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.linkIcons.Name = "linkIcons";
            this.linkIcons.TabStop = true;
            this.linkIcons.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkIcons_LinkClicked);
            // 
            // labelIcons
            // 
            resources.ApplyResources(this.labelIcons, "labelIcons");
            this.labelIcons.Name = "labelIcons";
            // 
            // linkLabelCC
            // 
            resources.ApplyResources(this.linkLabelCC, "linkLabelCC");
            this.linkLabelCC.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.linkLabelCC.Name = "linkLabelCC";
            this.linkLabelCC.TabStop = true;
            this.linkLabelCC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCC_LinkClicked);
            // 
            // About
            // 
            resources.ApplyResources(this, "$this");
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.latestVersionBox);
            this.Controls.Add(this.linkLabelCC);
            this.Controls.Add(this.linkIcons);
            this.Controls.Add(this.labelIcons);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.linkLabelExplorer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.linkLabelChocolatey);
            this.Controls.Add(this.label1);
            this.Name = "About";
            this.Load += new System.EventHandler(this.About_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox latestVersionBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabelChocolatey;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabelExplorer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.LinkLabel linkIcons;
        private System.Windows.Forms.Label labelIcons;
        private System.Windows.Forms.LinkLabel linkLabelCC;
    }
}