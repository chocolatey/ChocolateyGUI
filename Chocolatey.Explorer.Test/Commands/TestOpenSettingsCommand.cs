using Chocolatey.Explorer.Commands;
using Chocolatey.Explorer.View.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Chocolatey.Explorer.Test.Commands
{
    public class TestOpenSettingsCommand
    {
        [Test]
        public void DoesDoShowOnExecute()
        {
            var command = new OpenSettingsCommand();
            var settings = MockRepository.GenerateMock<ISettings>();
            command.Settings = settings;
            command.Execute();
            settings.AssertWasCalled(x => x.DoShowDialog());
        } 
    }
}