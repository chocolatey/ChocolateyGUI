using System.Collections.Generic;
using System.Windows.Forms;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.LogsService;

namespace Chocolatey.Explorer.View.Forms
{
    public partial class Logs : Form, ILogs
    {
        private readonly ILogsService _logsService;

        public Logs(ILogsService logsService)
        {
            _logsService = logsService;
            _logsService.RunFinished += RunFinished;
            InitializeComponent();
        }

        private void RunFinished(IList<string> logs)
        {
            foreach (var log in logs)
            {
                listBox1.Items.Add(log);
            }
        }

        public void DoShow()
        {
            Show();
            _logsService.GetLogs();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process.Start(listBox1.SelectedItem.ToString());
        }
    }
}
