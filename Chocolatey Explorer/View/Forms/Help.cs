using System.Windows.Forms;
using Chocolatey.Explorer.Services;
using log4net;
using System.ComponentModel;

namespace Chocolatey.Explorer.View.Forms
{
    public partial class Help : Form,IHelp
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Help));

        private delegate void ChocolateyServiceOutPutHandler(string output);
        private IChocolateyService _chocolateyService;

        public Help() : this(new ChocolateyService())
        {
        }

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
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(
                delegate(object o, DoWorkEventArgs args)
                {
                    _chocolateyService.Help();
                }
            );
            bw.RunWorkerAsync();
        }

        public void DoShow()
        {
            this.Show();
        }
    }
}
