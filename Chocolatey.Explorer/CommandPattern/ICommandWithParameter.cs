namespace Chocolatey.Explorer.CommandPattern
{
    public interface ICommandWithParameter<in TParameterType>
    {
        void ExecuteWitParameter(TParameterType parameter);
    }
}