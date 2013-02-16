using StructureMap;

namespace Chocolatey.Explorer.CommandPattern
{
    public abstract class BaseCommand:ICommand
    {
        protected BaseCommand()
        {
            ObjectFactory.BuildUp(this);
        }

        public abstract void Execute();
    }
}