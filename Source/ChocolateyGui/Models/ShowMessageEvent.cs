using System;

namespace ChocolateyGui.Models
{
    public delegate void ShowMessageEventHandler(object sender, ShowMessageEventArgs e);

    public class ShowMessageEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
