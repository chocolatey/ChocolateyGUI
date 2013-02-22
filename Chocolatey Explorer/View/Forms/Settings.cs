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

        /// <summary>
        /// Close form on escape key.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) this.Close();
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
