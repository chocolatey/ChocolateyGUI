// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocoWamp.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using ChocolateyGui.Models;
using WampSharp.Binding;
using WampSharp.Fleck;
using WampSharp.V2;
using WampSharp.V2.Realm;

namespace ChocolateyGui.Subprocess
{
    public class ChocoWamp
    {
        public ChocoWamp(int port)
        {
            Location = $"ws://127.0.0.1:{port}/ws";
        }

        private string Location { get;  }

        private WampHost Host { get; set; }

        public async Task Start()
        {
            Host = new WampHost();
            Host.RegisterTransport(new FleckWebSocketTransport(Location), new JTokenJsonBinding());
            var realm = Host.RealmContainer.GetRealmByName("default");
            var chocoService = new ChocolateyService(realm.Services.GetSubject<StreamingLogMessage>("com.chocolatey.log"));
            await realm.Services.RegisterCallee(chocoService);

            // When the starting service has closed, for any reason, exit this server.
            // We should never outlive out parent process.
            realm.SessionClosed += RealmOnSessionClosed;

            Host.Open();
        }

        private static void RealmOnSessionClosed(object sender, WampSessionCloseEventArgs eventArgs)
        {
            EndServer();
        }

        private static void EndServer()
        {
            Program.CanceledEvent.Set();
        }
    }
}