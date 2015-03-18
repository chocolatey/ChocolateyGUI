// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackagesChangedEventManager.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities
{
    using System.Windows;
    using ChocolateyGui.Models;
    using ChocolateyGui.Services;
    
    public class PackagesChangedEventManager : WeakEventManager
    {
        private static PackagesChangedEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(PackagesChangedEventManager);
                var manager = (PackagesChangedEventManager)GetCurrentManager(managerType);
                if (manager != null)
                {
                    return manager;
                }

                manager = new PackagesChangedEventManager();
                PackagesChangedEventManager.SetCurrentManager(managerType, manager);
                return manager;
            }
        }

        public static void AddListener(IChocolateyPackageService service, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(service, listener);
        }

        public static void RemoveListener(IChocolateyPackageService service, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(service, listener);
        }

        protected override void StartListening(object source)
        {
            (source as IChocolateyPackageService).PackagesUpdated += this.OnPackagedUpdated;
        }

        protected override void StopListening(object source)
        {
            (source as IChocolateyPackageService).PackagesUpdated -= this.OnPackagedUpdated;
        }

        private void OnPackagedUpdated(object sender, PackagesChangedEventArgs e)
        {
            this.DeliverEvent(sender, e);
        }
    }
}