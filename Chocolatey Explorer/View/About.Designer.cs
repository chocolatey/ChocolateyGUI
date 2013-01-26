﻿namespace Chocolatey.Explorer.View
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
            this.latestVersionBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.latestVersionBox.Location = new System.Drawing.Point(0, 191);
            this.latestVersionBox.Multiline = true;
            this.latestVersionBox.Name = "latestVersionBox";
            this.latestVersionBox.Size = new System.Drawing.Size(592, 126);
            this.latestVersionBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(280, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Chocolatey was written by Rob Reynolds (@ferventcoder)";
            // 
            // linkLabelChocolatey
            // 
            this.linkLabelChocolatey.AutoSize = true;
            this.linkLabelChocolatey.Location = new System.Drawing.Point(21, 29);
            this.linkLabelChocolatey.Name = "linkLabelChocolatey";
            this.linkLabelChocolatey.Size = new System.Drawing.Size(113, 13);
            this.linkLabelChocolatey.TabIndex = 2;
            this.linkLabelChocolatey.TabStop = true;
            this.linkLabelChocolatey.Text = "http://chocolatey.org/";
            this.linkLabelChocolatey.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelChocolatey_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(305, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Chocolatey explorer was written by christiaan baes (@chrissie1)";
            // 
            // linkLabelExplorer
            // 
            this.linkLabelExplorer.AutoSize = true;
            this.linkLabelExplorer.Location = new System.Drawing.Point(21, 68);
            this.linkLabelExplorer.Name = "linkLabelExplorer";
            this.linkLabelExplorer.Size = new System.Drawing.Size(239, 13);
            this.linkLabelExplorer.TabIndex = 4;
            this.linkLabelExplorer.TabStop = true;
            this.linkLabelExplorer.Text = "https://github.com/chrissie1/chocolatey-Explorer";
            this.linkLabelExplorer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelExplorer_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Version";
            // 
            // lblVersion
            // 
            this.lblVersion.Location = new System.Drawing.Point(26, 167);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(100, 23);
            this.lblVersion.TabIndex = 6;
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Location = new System.Drawing.Point(0, 317);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(592, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 7;
            this.progressBar.Visible = false;
            // 
            // linkIcons
            // 
            this.linkIcons.AutoSize = true;
            this.linkIcons.Location = new System.Drawing.Point(21, 107);
            this.linkIcons.Name = "linkIcons";
            this.linkIcons.Size = new System.Drawing.Size(176, 13);
            this.linkIcons.TabIndex = 9;
            this.linkIcons.TabStop = true;
            this.linkIcons.Text = "https://www.fatcow.com/free-icons";
            this.linkIcons.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkIcons_LinkClicked);
            // 
            // labelIcons
            // 
            this.labelIcons.AutoSize = true;
            this.labelIcons.Location = new System.Drawing.Point(21, 94);
            this.labelIcons.Name = "labelIcons";
            this.labelIcons.Size = new System.Drawing.Size(86, 13);
            this.labelIcons.TabIndex = 8;
            this.labelIcons.Text = "Icons by FatCow";
            // 
            // linkLabelCC
            // 
            this.linkLabelCC.AutoSize = true;
            this.linkLabelCC.Location = new System.Drawing.Point(200, 107);
            this.linkLabelCC.Name = "linkLabelCC";
            this.linkLabelCC.Size = new System.Drawing.Size(80, 13);
            this.linkLabelCC.TabIndex = 10;
            this.linkLabelCC.TabStop = true;
            this.linkLabelCC.Text = "(CC BY 3.0 US)";
            this.linkLabelCC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCC_LinkClicked);
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 340);
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "About";
            this.Text = "About";
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