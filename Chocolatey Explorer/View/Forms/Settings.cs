using System;
using System.Windows.Forms;
using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.Commands;
using Chocolatey.Explorer.Services.SettingsService;

namespace Chocolatey.Explorer.View.Forms
{
    public partial class Settings : Form, ISettings
    {
        private readonly ISettingsService _settingsService;
        private readonly ICommandExecuter _commandExecutor;

        public Settings(ISettingsService settingsService, ICommandExecuter commandExecutor)
        {
            _settingsService = settingsService;
            _commandExecutor = commandExecutor;
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            txtInstallDirectory.Text = _settingsService.ChocolateyLibDirectory;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _settingsService.ChocolateyLibDirectory = txtInstallDirectory.Text;
            MessageBox.Show("Chocolatey lib directory saved to settings.", "Lib directory", MessageBoxButtons.OK);
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

        private void btnClearPackageVersionCache_Click(object sender, EventArgs e)
        {
            _commandExecutor.Execute<ClearCachePackageVersionCommand>();
            MessageBox.Show("Cache package version is cleared.", "Cache", MessageBoxButtons.OK);
        }

        private void btnClearInstalledPackagesCache_Click(object sender, EventArgs e)
        {
            _commandExecutor.Execute<ClearCacheInstalledPackagesCommand>();
            MessageBox.Show("Cache installed packages is cleared.", "Cache", MessageBoxButtons.OK);
        }

        private void btnClearAvailablePackagesCache_Click(object sender, EventArgs e)
        {
            _commandExecutor.Execute<ClearCacheAvailablePackagesCommand>();
            MessageBox.Show("Cache available packages is cleared.", "Cache", MessageBoxButtons.OK);
        }

        private void btnClearCacheAll_Click(object sender, EventArgs e)
        {
            _commandExecutor.Execute<ClearCacheAllCommand>();
            MessageBox.Show("Cache is cleared.", "Cache", MessageBoxButtons.OK);
        }

    }
}
