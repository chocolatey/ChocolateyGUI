using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Chocolatey.Explorer.View
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            var settings = new Properties.Settings();
            txtInstallDirectory.Text = settings.Installdirectory;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var settings = new Properties.Settings();
            settings.Installdirectory = txtInstallDirectory.Text;
            settings.Save();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var dirDialog = new FolderBrowserDialog();
            if(dirDialog.ShowDialog() == DialogResult.OK)
            {
                txtInstallDirectory.Text = dirDialog.SelectedPath;
            }
        }
    }
}
