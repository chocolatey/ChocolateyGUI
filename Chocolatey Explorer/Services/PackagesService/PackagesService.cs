using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Powershell;
using log4net;

namespace Chocolatey.Explorer.Services
{
    public class PackagesService : IPackagesService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PackagesService));

        private readonly IRun _powershellAsync;
        private readonly IList<string> _lines;
        private readonly ISourceService _sourceService;
        private readonly ChocolateyLibDirHelper _libDirHelper;

        public delegate void FinishedDelegate(IList<Package> packages);
        public event FinishedDelegate RunFinshed;

        public PackagesService(): this(new RunAsync(), new SourceService())
        {
        }

        public PackagesService(IRun powershell, ISourceService sourceService)
        {
            _lines = new List<string>();
            _sourceService = sourceService;
            _libDirHelper = new ChocolateyLibDirHelper();
            _powershellAsync = powershell;
            _powershellAsync.OutputChanged += OutputChanged;
            _powershellAsync.RunFinished += RunFinished;
        }

        public void ListOfPackages()
        {
            log.Info("Getting list of packages on source: " + _sourceService.Source);
            _powershellAsync.Run("clist -source " + _sourceService.Source);
        }

        public void ListOfInstalledPackages()
        {
            log.Info("Getting list of installed packages");
            var thread = new Thread(ListOfInstalledPackagsThread) {IsBackground = true};
            thread.Start();
        }

        private  void ListOfInstalledPackagsThread()
        {
            OnRunFinshed(_libDirHelper.ReloadFromDir());
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
                    select new Package { Name = name }).ToList());
        }

        private void OnRunFinshed(IList<Package> packages)
        {
            FinishedDelegate handler = RunFinshed;
            if (handler != null) handler(packages);
        }

    }
}