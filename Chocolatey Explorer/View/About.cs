using System;
using System.Windows.Forms;
using Chocolatey_Explorer.Services;

namespace Chocolatey_Explorer.View
{
    public partial class About : Form
    {
        private Chocolatey _chocolateyService;

        public About()
        {
            InitializeComponent();

            _chocolateyService = new Chocolatey();
            _chocolateyService.OutputChanged += VersionChangedHandler;
        }

        private void VersionChangedHandler(string version)
        {
            textBox1.Text = version;
        }

        public About(Chocolatey chocolateyService)
        {
            InitializeComponent();

            _chocolateyService = chocolateyService;
        }

        private void About_Activated(object sender, EventArgs e)
        {
            _chocolateyService.LatestVersion();
        }
    }
}
