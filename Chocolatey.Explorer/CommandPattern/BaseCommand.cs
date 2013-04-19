using StructureMap;

namespace Chocolatey.Explorer.CommandPattern
{
    /// <summary>
    /// Inherits from ICommand and filles the properties via the IoC container.
    /// </summary>
    public abstract class BaseCommand:ICommand
    {
        protected BaseCommand()
        {
            ObjectFactory.BuildUp(this);
        }

        /// <summary>
        /// Mustoverride execute method so that the executer has something to execute.
        /// </summary>
        public abstract void Execute();
    }
}