using System.Collections.Generic;
using System.Linq;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services.SourceService;

namespace Chocolatey.Explorer.Services.PackagesService
{
    public class AvailablePackagesService : IAvailablePackagesService
    {
        private readonly IRun _powershellAsync;
        private readonly IList<string> _lines;
        private readonly ISourceService _sourceService;
        
        public event Delegates.FinishedDelegate RunFinshed;
        public event Delegates.FailedDelegate RunFailed;
        public event Delegates.StartedDelegate RunStarted;

        public AvailablePackagesService()
            : this(new RunAsync(), new SourceService.SourceService())
        {
        }

        public AvailablePackagesService(IRunAsync powershell, ISourceService sourceService)
        {
            _lines = new List<string>();
            _sourceService = sourceService;
            _powershellAsync = powershell;
            _powershellAsync.OutputChanged += OutputChanged;
            _powershellAsync.RunFinished += RunFinished;
        }

        public void ListOfAvalablePackages()
        {
            this.Log().Info("Getting list of packages on source: " + _sourceService.Source);
            OnRunStarted();
            _powershellAsync.Run("clist -source " + _sourceService.Source);
        }

        private void OutputChanged(string line)
        {
            _lines.Add(line);
        }

        private void RunFinished()
        {
            OnRunFinshed((from result in _lines
                    let name = result.Split(" ".ToCharArray()[0])[0]
                    let version = result.Split(" ".ToCharArray()[0])[1]
                    select new Package { Name = name, InstalledVersion = version}).ToList());
        }

        private void OnRunFinshed(IList<Package> packages)
        {
            var handler = RunFinshed;
            if (handler != null) handler(packages);
        }

        private void OnRunStarted()
        {
            var handler = RunStarted;
            if (handler != null) handler("Getting list of available packages.");
        }

    }
}