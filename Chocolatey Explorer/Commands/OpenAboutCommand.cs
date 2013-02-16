using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.View;

namespace Chocolatey.Explorer.Commands
{
    public class OpenAboutCommand : ICommand
    {
        public void Execute()
        {
            var about = new About();
            about.ShowDialog();
        }
    }
}