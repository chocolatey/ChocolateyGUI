using System;
using System.Windows.Forms;

namespace Chocolatey.Explorer.View.Forms
{
    public partial class Settings : Form, ISettings
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            var settings = new Properties.Settings();
            txtInstallDirectory.Text = settings.ChocolateyLibDirectory;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var settings = new Properties.Settings();
            settings.ChocolateyLibDirectory = txtInstallDirectory.Text;
            settings.Save();
            Dispose();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var dirDialog = new FolderBrowserDialog();
            if(dirDialog.ShowDialog() == DialogResult.OK)
            {
                txtInstallDirectory.Text = dirDialog.SelectedPath;
            }
        }

        public void DoShowDialog()
        {
            this.ShowDialog();
        }
    }
}
