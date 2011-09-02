using System;
using System.Windows.Forms;
using Chocolatey.Explorer.Services;

namespace Chocolatey.Explorer.View
{
    public partial class About : Form
    {
        private ChocolateyService _chocolateyService;

        public About() : this(new ChocolateyService())
        {
        }

        public About(ChocolateyService chocolateyService)
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
    }
}
