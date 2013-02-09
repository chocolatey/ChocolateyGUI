using System;
using System.Reflection;
using System.Windows.Forms;
using Chocolatey.Explorer.Services;
using log4net;
using System.ComponentModel;

namespace Chocolatey.Explorer.View
{
    public partial class About : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(About));

        private delegate void VersionChangedHandler(string version);
        private IChocolateyService _chocolateyService;

        public About() : this(new ChocolateyService())
        {
        }

        public About(IChocolateyService chocolateyService)
        {
            InitializeComponent();

            _chocolateyService = chocolateyService;
            _chocolateyService.OutputChanged += VersionChangeFinished;

            GetChocolateyVersionAsync();
        }

        private void VersionChangeFinished(string version)
        {
            if (this.InvokeRequired)
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

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(
                delegate(object o, DoWorkEventArgs args)
                {
                    _chocolateyService.LatestVersion();
                }
            );
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
    }
}
