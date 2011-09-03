using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Powershell;

namespace Chocolatey.Explorer.Services
{
    public class PackagesService : IPackagesService
    {
        private IRun _powershellAsync;
        private IList<string> lines;
 
        public delegate void FinishedDelegate(IList<Package> packages);
        public event FinishedDelegate RunFinshed;

        private void OnRunFinshed(IList<Package> packages)
        {
            FinishedDelegate handler = RunFinshed;
            if (handler != null) handler(packages);
        }

        public PackagesService(): this(new RunAsync())
        {
        }

        public PackagesService(IRun powershell)
        {
            lines = new List<string>();
            _powershellAsync = powershell;
            _powershellAsync.OutputChanged += OutputChanged;
            _powershellAsync.RunFinished += RunFinished;
        }

        public void ListOfPackages()
        {
            _powershellAsync.Run("clist -source " + Settings.Source);
        }

        public void ListOfInstalledPackages()
        {
            var thread = new Thread(ListOfInstalledPackagsThread);
            thread.IsBackground = true;
            thread.Start();
        }

        private  void ListOfInstalledPackagsThread()
        {
            var folders = System.IO.Directory.GetDirectories("c:/nuget/lib");
            IList<Package> packages = new List<Package>();
            foreach (var folder in folders)
            {
                var folder2 = folder.Split("\\".ToCharArray())[1];
                var name = folder2.Substring(0, folder2.IndexOf("."));
                packages.Add(new Package() { Name = name });
            }
            OnRunFinshed(packages);
        }

        private void OutputChanged(string line)
        {
            lines.Add(line);
        }

        private void RunFinished()
        {
            OnRunFinshed((from result in lines
                    let name = result.Split(" ".ToCharArray()[0])[0]
                    let version = result.Split(" ".ToCharArray()[0])[1]
                    select new Package() { Name = name }).ToList());
        }
        
    }
}