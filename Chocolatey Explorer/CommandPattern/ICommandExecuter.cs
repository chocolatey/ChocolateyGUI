using System;

namespace Chocolatey.Explorer.CommandPattern
{
    public interface ICommandExecuter
    {
        void Execute<TCommand>()where TCommand : class, ICommand, new();
        void Execute<TCommandWithParameter, TParameterType>(TParameterType parameter) where TCommandWithParameter : class, ICommandWithParameter<Object>, new();
    }
}