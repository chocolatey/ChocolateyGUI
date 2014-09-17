using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChocolateyGui.Models
{
    public delegate void PackagesChangedEventHandler(object sender, PackagesChangedEventArgs e);
   
    public class PackagesChangedEventArgs : EventArgs
    {
        public PackagesChangedEventType EventType { get; set; }
        public string PackageId { get; set; }
        public string PackageVersion { get; set; }
    }

    public enum PackagesChangedEventType
    {
        Installed,
        Uninstalled,
        Updated
    }
}
