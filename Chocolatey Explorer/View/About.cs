using System;
using System.Windows.Forms;
using Chocolatey.Explorer.Services;

namespace Chocolatey.Explorer.View
{
    public partial class About : Form
    {
        private ChocolateyService _chocolateyService;

        public About()
        {
            InitializeComponent();

            _chocolateyService = new ChocolateyService();
            _chocolateyService.OutputChanged += VersionChangedHandler;
        }

        private void VersionChangedHandler(string version)
        {
            textBox1.Text = version;
        }

        public About(ChocolateyService chocolateyService)
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
