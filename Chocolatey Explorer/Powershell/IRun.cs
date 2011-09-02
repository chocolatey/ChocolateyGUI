using System;
using System.Collections.Generic;
using System.Text;

namespace Chocolatey_Explorer.Powershell
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