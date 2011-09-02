using System;
using System.Windows.Forms;
using Chocolatey.Explorer.Services;

namespace Chocolatey.Explorer.View
{
    public partial class Help : Form
    {
        private ChocolateyService _chocolateyService;

        public Help() : this(new ChocolateyService())
        {
        }

        private void ChocolateyServiceOutPutChanged(string output)
        {
            textBox1.Text = output;
        }

        public Help(ChocolateyService chocolateyService)
        {
            InitializeComponent();

            _chocolateyService = chocolateyService;
            _chocolateyService.OutputChanged += ChocolateyServiceOutPutChanged;
        }

        private void Help_Activated(object sender, EventArgs e)
        {
            _chocolateyService.Help();
        }

    }
}
