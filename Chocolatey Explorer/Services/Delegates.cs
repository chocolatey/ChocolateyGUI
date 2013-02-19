using System;
using System.Collections.Generic;
using Chocolatey.Explorer.Model;

namespace Chocolatey.Explorer.Services
{
    public class Delegates
    {
        public delegate void FinishedDelegate(IList<Package> packages);
        public delegate void FailedDelegate(Exception exc);
        public delegate void VersionResult(PackageVersion version);
        public delegate void StartedDelegate();
    }
}