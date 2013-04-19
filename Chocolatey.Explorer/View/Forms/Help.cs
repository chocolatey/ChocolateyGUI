using System.Windows.Forms;
using Chocolatey.Explorer.Extensions;
using System.ComponentModel;
using Chocolatey.Explorer.Services.ChocolateyService;

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
            this.Invoke(() =>
                {
                    progressBar.Visible = false;
                    textBox1.Text = output;
                });
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
