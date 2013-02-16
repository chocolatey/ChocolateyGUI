using System;

namespace Chocolatey.Explorer.CommandPattern
{
    public class CommandExecuter : ICommandExecuter
    {
        public void Execute<TCommand>()where TCommand : class, ICommand, new()
        {
            var command = new TCommand();
            command.Execute();
        }

        public void Execute<TCommandWithParameter, TParameterType>(TParameterType parameter) where TCommandWithParameter : class, ICommandWithParameter<Object>, new()
        {
            var command = new TCommandWithParameter();
            command.ExecuteWitParameter(parameter);
        }
    }
}