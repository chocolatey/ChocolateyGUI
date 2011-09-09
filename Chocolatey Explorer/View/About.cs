using System;
using System.Windows.Forms;
using Chocolatey.Explorer.Services;
using log4net;

namespace Chocolatey.Explorer.View
{
    public partial class About : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(About));

        private IChocolateyService _chocolateyService;

        public About() : this(new ChocolateyService())
        {
        }

        public About(IChocolateyService chocolateyService)
        {
            InitializeComponent();

            _chocolateyService = chocolateyService;
            _chocolateyService.OutputChanged += VersionChangedHandler;
        }

        private void VersionChangedHandler(string version)
        {
            textBox1.Text = version;
        }

        

        private void About_Activated(object sender, EventArgs e)
        {
            _chocolateyService.LatestVersion();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel2.LinkVisited = true;
            System.Diagnostics.Process.Start(linkLabel2.Text);
        }
    }
}
