using System;
using System.Windows.Forms;
using Chocolatey_Explorer.Services;

namespace Chocolatey_Explorer.View
{
    public partial class Help : Form
    {
        private Chocolatey _chocolateyService;

        public Help()
        {
            InitializeComponent();

            _chocolateyService = new Chocolatey();
            _chocolateyService.OutputChanged += ChocolateyServiceOutPutChanged;
        }

        private void ChocolateyServiceOutPutChanged(string output)
        {
            textBox1.Text = output;
        }

        public Help(Chocolatey chocolateyService)
        {
            InitializeComponent();

            _chocolateyService = chocolateyService;
        }

        private void Help_Activated(object sender, EventArgs e)
        {
            _chocolateyService.Help();
        }

    }
}
