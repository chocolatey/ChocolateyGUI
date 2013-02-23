using Chocolatey.Explorer.Commands;
using Chocolatey.Explorer.View.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Chocolatey.Explorer.Test.Commands
{
    public class TestLogsCommand
    {
        [Test]
        public void DoesDoShowOnExecute()
        {
            var command = new OpenLogsCommand();
            var logs = MockRepository.GenerateMock<ILogs>();
            command.Logs = logs;
            command.Execute();
            logs.AssertWasCalled(x => x.DoShow());
        }
    }
}