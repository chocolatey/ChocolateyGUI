// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackagesChangedEventManager.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities
{
    using ChocolateyGui.Models;
    using ChocolateyGui.Services;
    using System.Windows;

    public class PackagesChangedEventManager : WeakEventManager
    {
        public static void AddListener(IChocolateyService serivce, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(serivce, listener);
        }
        public static void RemoveListener(IChocolateyService serivce, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(serivce, listener);
        }

        private static PackagesChangedEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(PackagesChangedEventManager);
                var manager = (PackagesChangedEventManager)GetCurrentManager(managerType);
                if (manager != null) 
                    return manager;

                manager = new PackagesChangedEventManager();
                SetCurrentManager(managerType, manager);
                return manager;
            }
        }

        protected override void StartListening(object source)
        {
            (source as IChocolateyService).PackagesUpdated += OnPackagedUpdated;
        }

        protected override void StopListening(object source)
        {
            (source as IChocolateyService).PackagesUpdated -= OnPackagedUpdated;
        }

        void OnPackagedUpdated(object sender, PackagesChangedEventArgs e)
        {
            DeliverEvent(sender, e);
        }
    }
}