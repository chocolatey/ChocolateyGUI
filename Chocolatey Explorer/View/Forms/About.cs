using System;
using System.Reflection;
using System.Windows.Forms;
using Chocolatey.Explorer.Services;
using System.ComponentModel;
using Chocolatey.Explorer.Services.ChocolateyService;

namespace Chocolatey.Explorer.View.Forms
{
    public partial class About : Form, IAbout
    {
        private delegate void VersionChangedHandler(string version);
        private readonly IChocolateyService _chocolateyService;

        public About(IChocolateyService chocolateyService)
        {
            InitializeComponent();

            _chocolateyService = chocolateyService;
            _chocolateyService.OutputChanged += VersionChangeFinished;

            GetChocolateyVersionAsync();
        }

        private void VersionChangeFinished(string version)
        {
            if (InvokeRequired)
            {
                Invoke(new VersionChangedHandler(VersionChangeFinished), new object[] { version });
            }
            else
            {
                latestVersionBox.Text = version;
                progressBar.Visible = false;
            }
        }

        private void GetChocolateyVersionAsync()
        {
            progressBar.Visible = true;

            var bw = new BackgroundWorker();
            bw.DoWork += (o, args) => _chocolateyService.LatestVersion();
            bw.RunWorkerAsync();
        }

        private void linkLabelChocolatey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelChocolatey.LinkVisited = true;
            System.Diagnostics.Process.Start(linkLabelChocolatey.Text);
        }

        private void linkLabelExplorer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelExplorer.LinkVisited = true;
            System.Diagnostics.Process.Start(linkLabelExplorer.Text);
        }

        private void linkIcons_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkIcons.LinkVisited = true;
            System.Diagnostics.Process.Start(linkIcons.Text);
        }

        private void linkLabelCC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelCC.LinkVisited = true;
            System.Diagnostics.Process.Start("https://creativecommons.org/licenses/by/3.0/us/");
        }

        private void About_Load(object sender, EventArgs e)
        {
            lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public void DoShow()
        {
            this.Show();
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
