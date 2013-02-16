using Chocolatey.Explorer.Commands;
using Chocolatey.Explorer.View.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Chocolatey.Explorer.Test.Commands
{
    public class TestOpenAboutCommand
    {
        [Test]
        public void DoesDoShowOnExecute()
        {
            var command = new OpenAboutCommand();
            var about = MockRepository.GenerateMock<IAbout>();
            command.About = about;
            command.Execute();
            about.AssertWasCalled(x => x.DoShow());
        }
    }
}