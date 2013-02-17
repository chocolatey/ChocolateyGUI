using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Chocolatey.Explorer.View.Controls
{
    partial class PackageButtonsPanel
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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackageButtonsPanel));
            this.installUninstallImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // installUninstallImageList
            // 
            this.installUninstallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("installUninstallImageList.ImageStream")));
            this.installUninstallImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.installUninstallImageList.Images.SetKeyName(0, "add.png");
            this.installUninstallImageList.Images.SetKeyName(1, "delete.png");
            // 
            // PackageButtonsPanel
            // 
            this.Name = "PackageButtonsPanel";
            this.ResumeLayout(false);
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnInstallUninstall = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnUpdate
            // 
            this.btnUpdate.AccessibleDescription = "Update selected package";
            this.btnUpdate.AccessibleName = "Update";
            this.btnUpdate.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUpdate.Enabled = false;
            this.btnUpdate.Image = global::Chocolatey.Explorer.Properties.Resources.update;
            this.btnUpdate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnUpdate.Location = new System.Drawing.Point(262, 3);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(254, 144);
            this.btnUpdate.TabIndex = 7;
            this.btnUpdate.Text = "Up&date";
            this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.BtnUpdateClick);
            // 
            // btnInstallUninstall
            // 
            this.btnInstallUninstall.AccessibleName = "";
            this.btnInstallUninstall.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnInstallUninstall.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnInstallUninstall.AutoCheck = false;
            this.btnInstallUninstall.Enabled = false;
            this.btnInstallUninstall.AutoSize = true;
            this.btnInstallUninstall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnInstallUninstall.ImageList = installUninstallImageList;
            this.btnInstallUninstall.ImageIndex = 0;
            this.btnInstallUninstall.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnInstallUninstall.Location = new System.Drawing.Point(3, 3);
            this.btnInstallUninstall.Name = "btnInstallUninstall";
            this.btnInstallUninstall.Size = new System.Drawing.Size(253, 144);
            this.btnInstallUninstall.TabIndex = 8;
            this.btnInstallUninstall.Text = "Install";
            this.btnInstallUninstall.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnInstallUninstall.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnInstallUninstall.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnInstallUninstall, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnUpdate, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(519, 150);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // PackageButtonsPanel
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PackageButtonsPanel";
            this.Size = new System.Drawing.Size(519, 150);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private Button btnUpdate;
        private TableLayoutPanel tableLayoutPanel1;
        private CheckBox btnInstallUninstall;
        private ImageList installUninstallImageList;
    }
}
