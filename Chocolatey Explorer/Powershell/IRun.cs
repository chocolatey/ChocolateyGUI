using System;

namespace Chocolatey.Explorer.Powershell
{
    public delegate void ResultsHandler(String result);
    public delegate void EmptyHandler();

    public interface IRun
    {
        void Run(String command);
        event ResultsHandler OutputChanged;
        event EmptyHandler RunFinished;
    }
}