namespace Chocolatey.Explorer.CommandPattern
{
    /// <summary>
    /// The commandexecuter is an easy way to execute the commands with generics.
    ///  </summary>
    /// <example>
    /// WithoutParameter
    /// _commandexecuter.Execute&lt;YourCommand&gt;();
    /// In this case Yourcommand has to implement <see cref="ICommand"/> or inherit <see cref="BaseCommand"/>.
    /// 
    /// WithParameter
    /// _commandExecuter.Execute&lt;YourCommand, string&gt;("value");
    /// In this case YourCommand has to implement <see cref="ICommandWithParameter"/>. 
    /// </example>
    public class CommandExecuter : ICommandExecuter
    {
        public void Execute<TCommand>()where TCommand : class, ICommand, new()
        {
            var command = new TCommand();
            command.Execute();
        }

        public void Execute<TCommandWithParameter, TParameterType>(TParameterType parameter) where TCommandWithParameter : class, ICommandWithParameter<TParameterType>, new()
        {
            var command = new TCommandWithParameter();
            command.ExecuteWitParameter(parameter);
        }
    }
}