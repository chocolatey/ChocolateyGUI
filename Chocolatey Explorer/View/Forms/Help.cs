using System.Windows.Forms;
using Chocolatey.Explorer.Services;
using System.ComponentModel;

namespace Chocolatey.Explorer.View.Forms
{
    public partial class Help : Form,IHelp
    {

        private delegate void ChocolateyServiceOutPutHandler(string output);
        private readonly IChocolateyService _chocolateyService;

        public Help(IChocolateyService chocolateyService)
        {
            InitializeComponent();

            _chocolateyService = chocolateyService;
            _chocolateyService.OutputChanged += ChocolateyServiceOutPutChanged;

            LoadHelp();
        }

        private void ChocolateyServiceOutPutChanged(string output)
        {
            if (this.InvokeRequired)
            {
                Invoke(new ChocolateyServiceOutPutHandler(ChocolateyServiceOutPutChanged), new object[] { output });
            }
            else
            {
                progressBar.Visible = false;
                textBox1.Text = output;
            }
        }

        private void LoadHelp()
        {
            progressBar.Visible = true;
            var bw = new BackgroundWorker();
            bw.DoWork += (o, args) => _chocolateyService.Help();
            bw.RunWorkerAsync();
        }

        public void DoShow()
        {
            this.Show();
        }
    }
}
