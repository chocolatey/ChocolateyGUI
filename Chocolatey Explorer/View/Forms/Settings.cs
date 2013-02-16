using System;
using System.Windows.Forms;
using Chocolatey.Explorer.Services.SettingsService;

namespace Chocolatey.Explorer.View.Forms
{
    public partial class Settings : Form, ISettings
    {
        private readonly ISettingsService _settingsService;
            
        public Settings(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            txtInstallDirectory.Text = _settingsService.ChocolateyLibDirectory;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _settingsService.ChocolateyLibDirectory = txtInstallDirectory.Text;
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
