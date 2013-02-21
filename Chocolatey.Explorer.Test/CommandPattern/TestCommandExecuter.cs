using System;
using Chocolatey.Explorer.CommandPattern;
using NUnit.Framework;

namespace Chocolatey.Explorer.Test.CommandPattern
{
    public class TestCommandExecuter
    {
        [Test]
        public void IfExecuteExecutesCommand()
        {
            var executer = new CommandExecuter();
            Assert.Throws<System.NotImplementedException>(executer.Execute<TestCommand>);
        }

        private class TestCommand:ICommand
        {
            public void Execute()
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void IfExecuteExecutesCommandWithParameter()
        {
            var executer = new CommandExecuter();
            Assert.Throws<System.NotImplementedException>(() => executer.Execute<TestCommandWithParameter, String>("test"));
        }

        private class TestCommandWithParameter : ICommandWithParameter<object>
        {
            public void ExecuteWitParameter(object parameter)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}