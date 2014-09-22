// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourcesChangedEventManager.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities
{
    using System.Windows;
    using ChocolateyGui.Models;
    using ChocolateyGui.Services;


    public class SourcesChangedEventManager : WeakEventManager
    {
        public static void AddListener(ISourceService serivce, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(serivce, listener);
        }

        public static void RemoveListener(ISourceService serivce, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(serivce, listener);
        }

        private static SourcesChangedEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(SourcesChangedEventManager);
                var manager = (SourcesChangedEventManager)GetCurrentManager(managerType);
                if (manager != null) 
                    return manager;

                manager = new SourcesChangedEventManager();
                SetCurrentManager(managerType, manager);
                return manager;
            }
        }

        protected override void StartListening(object source)
        {
            (source as ISourceService).SourcesChanged += OnSourceUpdated;
        }

        protected override void StopListening(object source)
        {
            (source as ISourceService).SourcesChanged -= OnSourceUpdated;
        }

        void OnSourceUpdated(object sender, SourcesChangedEventArgs e)
        {
            DeliverEvent(sender, e);
        }
    }
}