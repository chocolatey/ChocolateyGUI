using Chocolatey.Explorer.Commands;
using Chocolatey.Explorer.View.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Chocolatey.Explorer.Test.Commands
{
    public class TestOpenHelpCommand
    {
        [Test]
        public void DoesDoShowOnExecute()
        {
            var command = new OpenHelpCommand();
            var help = MockRepository.GenerateMock<IHelp>();
            command.Help = help;
            command.Execute();
            help.AssertWasCalled(x => x.DoShow());
        } 
    }
}