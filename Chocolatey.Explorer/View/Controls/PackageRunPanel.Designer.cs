namespace Chocolatey.Explorer.View.Controls
{
    partial class PackageRunPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackageRunPanel));
            this.txtPowershellOutput = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.titleBar = new System.Windows.Forms.TableLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.picServerVersion = new System.Windows.Forms.PictureBox();
            this.lblServerVersion = new System.Windows.Forms.Label();
            this.picInstalledVersion = new System.Windows.Forms.PictureBox();
            this.lblInstalledVersion = new System.Windows.Forms.Label();
            this.versionBar = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.titleBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picServerVersion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInstalledVersion)).BeginInit();
            this.versionBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtPowershellOutput
            // 
            this.txtPowershellOutput.AccessibleName = "Console output";
            this.txtPowershellOutput.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.txtPowershellOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPowershellOutput.Font = new System.Drawing.Font("Courier New", 9F);
            this.txtPowershellOutput.Location = new System.Drawing.Point(0, 100);
            this.txtPowershellOutput.Multiline = true;
            this.txtPowershellOutput.Name = "txtPowershellOutput";
            this.txtPowershellOutput.ReadOnly = true;
            this.txtPowershellOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPowershellOutput.Size = new System.Drawing.Size(363, 213);
            this.txtPowershellOutput.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(363, 100);
            this.panel1.TabIndex = 9;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.titleBar, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.versionBar, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(363, 100);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // titleBar
            // 
            this.titleBar.AutoSize = true;
            this.titleBar.ColumnCount = 2;
            this.titleBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.titleBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.titleBar.Controls.Add(this.lblName, 0, 0);
            this.titleBar.Controls.Add(this.lblAuthor, 1, 0);
            this.titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleBar.Location = new System.Drawing.Point(0, 0);
            this.titleBar.Margin = new System.Windows.Forms.Padding(0);
            this.titleBar.Name = "titleBar";
            this.titleBar.RowCount = 1;
            this.titleBar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.titleBar.Size = new System.Drawing.Size(363, 50);
            this.titleBar.TabIndex = 7;
            // 
            // lblName
            // 
            this.lblName.AccessibleDescription = "title of the selected package";
            this.lblName.AccessibleName = "title";
            this.lblName.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblName.AutoSize = true;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblName.Location = new System.Drawing.Point(0, 0);
            this.lblName.Margin = new System.Windows.Forms.Padding(0);
            this.lblName.Name = "lblName";
            this.lblName.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.lblName.Size = new System.Drawing.Size(50, 50);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Title";
            // 
            // lblAuthor
            // 
            this.lblAuthor.AccessibleDescription = "author of the selected package";
            this.lblAuthor.AccessibleName = "author";
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblAuthor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblAuthor.Location = new System.Drawing.Point(50, 0);
            this.lblAuthor.Margin = new System.Windows.Forms.Padding(0);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(50, 17);
            this.lblAuthor.TabIndex = 1;
            this.lblAuthor.Text = "Author";
            this.lblAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // picServerVersion
            // 
            this.picServerVersion.Image = ((System.Drawing.Image)(resources.GetObject("picServerVersion.Image")));
            this.picServerVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.picServerVersion.Location = new System.Drawing.Point(0, 0);
            this.picServerVersion.Margin = new System.Windows.Forms.Padding(0);
            this.picServerVersion.Name = "picServerVersion";
            this.picServerVersion.Size = new System.Drawing.Size(32, 32);
            this.picServerVersion.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picServerVersion.TabIndex = 3;
            this.picServerVersion.TabStop = false;
            // 
            // lblServerVersion
            // 
            this.lblServerVersion.AccessibleDescription = "newest available version of the selected package";
            this.lblServerVersion.AccessibleName = "server version";
            this.lblServerVersion.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblServerVersion.AutoSize = true;
            this.lblServerVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblServerVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblServerVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblServerVersion.Location = new System.Drawing.Point(35, 0);
            this.lblServerVersion.Name = "lblServerVersion";
            this.lblServerVersion.Size = new System.Drawing.Size(140, 44);
            this.lblServerVersion.TabIndex = 2;
            this.lblServerVersion.Text = "server version";
            this.lblServerVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // picInstalledVersion
            // 
            this.picInstalledVersion.Image = global::Chocolatey.Explorer.Properties.Resources.monitor;
            this.picInstalledVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.picInstalledVersion.InitialImage = null;
            this.picInstalledVersion.Location = new System.Drawing.Point(178, 0);
            this.picInstalledVersion.Margin = new System.Windows.Forms.Padding(0);
            this.picInstalledVersion.Name = "picInstalledVersion";
            this.picInstalledVersion.Size = new System.Drawing.Size(32, 32);
            this.picInstalledVersion.TabIndex = 5;
            this.picInstalledVersion.TabStop = false;
            // 
            // lblInstalledVersion
            // 
            this.lblInstalledVersion.AccessibleDescription = "installed version of the selected package";
            this.lblInstalledVersion.AccessibleName = "installed version";
            this.lblInstalledVersion.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblInstalledVersion.AutoSize = true;
            this.lblInstalledVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblInstalledVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblInstalledVersion.Location = new System.Drawing.Point(213, 0);
            this.lblInstalledVersion.Name = "lblInstalledVersion";
            this.lblInstalledVersion.Size = new System.Drawing.Size(121, 20);
            this.lblInstalledVersion.TabIndex = 4;
            this.lblInstalledVersion.Text = "installed version";
            this.lblInstalledVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // versionBar
            // 
            this.versionBar.AutoSize = true;
            this.versionBar.ColumnCount = 4;
            this.versionBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.versionBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.versionBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.versionBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.versionBar.Controls.Add(this.picServerVersion, 0, 0);
            this.versionBar.Controls.Add(this.lblServerVersion, 1, 0);
            this.versionBar.Controls.Add(this.picInstalledVersion, 2, 0);
            this.versionBar.Controls.Add(this.lblInstalledVersion, 3, 0);
            this.versionBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.versionBar.Location = new System.Drawing.Point(3, 53);
            this.versionBar.Name = "versionBar";
            this.versionBar.RowCount = 1;
            this.versionBar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.versionBar.Size = new System.Drawing.Size(357, 44);
            this.versionBar.TabIndex = 6;
            // 
            // PackageRunPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtPowershellOutput);
            this.Controls.Add(this.panel1);
            this.Name = "PackageRunPanel";
            this.Size = new System.Drawing.Size(363, 313);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.titleBar.ResumeLayout(false);
            this.titleBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picServerVersion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInstalledVersion)).EndInit();
            this.versionBar.ResumeLayout(false);
            this.versionBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPowershellOutput;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel titleBar;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.TableLayoutPanel versionBar;
        private System.Windows.Forms.PictureBox picServerVersion;
        private System.Windows.Forms.Label lblServerVersion;
        private System.Windows.Forms.PictureBox picInstalledVersion;
        private System.Windows.Forms.Label lblInstalledVersion;
    }
}
