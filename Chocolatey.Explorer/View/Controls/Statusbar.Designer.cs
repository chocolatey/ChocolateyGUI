namespace Chocolatey.Explorer.View.Controls
{
    partial class Statusbar
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
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressbar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.progressbar2 = new System.Windows.Forms.ToolStripProgressBar();
            this.lblprogress = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.AccessibleDescription = "Status bar";
            this.statusStrip.AccessibleName = "Status bar";
            this.statusStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.StatusBar;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressbar1,
            this.progressbar2,
            this.lblprogress,
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 1);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(767, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // progressbar1
            // 
            this.progressbar1.AccessibleDescription = "Progress";
            this.progressbar1.AccessibleName = "Progress";
            this.progressbar1.AccessibleRole = System.Windows.Forms.AccessibleRole.ProgressBar;
            this.progressbar1.Name = "progressbar1";
            this.progressbar1.Size = new System.Drawing.Size(200, 16);
            this.progressbar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressbar1.Visible = false;
            // 
            // progressbar2
            // 
            this.progressbar2.AccessibleDescription = "Progress";
            this.progressbar2.AccessibleName = "Progress";
            this.progressbar2.AccessibleRole = System.Windows.Forms.AccessibleRole.ProgressBar;
            this.progressbar2.Name = "progressbar2";
            this.progressbar2.Size = new System.Drawing.Size(200, 16);
            this.progressbar2.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressbar2.Visible = false;
            // 
            // lblprogress
            // 
            this.lblprogress.AccessibleDescription = "Current status";
            this.lblprogress.AccessibleName = "Current status";
            this.lblprogress.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblprogress.Name = "lblprogress";
            this.lblprogress.Size = new System.Drawing.Size(714, 17);
            this.lblprogress.Spring = true;
            this.lblprogress.Text = "progress";
            this.lblprogress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            this.lblStatus.AccessibleDescription = "Current status";
            this.lblStatus.AccessibleName = "Current status";
            this.lblStatus.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(38, 17);
            this.lblStatus.Tag = "test2";
            this.lblStatus.Text = "status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Statusbar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.statusStrip);
            this.Name = "Statusbar";
            this.Size = new System.Drawing.Size(767, 23);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar progressbar1;
        private System.Windows.Forms.ToolStripProgressBar progressbar2;
        private System.Windows.Forms.ToolStripStatusLabel lblprogress;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}
