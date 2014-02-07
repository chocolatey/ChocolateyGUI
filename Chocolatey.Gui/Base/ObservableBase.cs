using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Chocolatey.Gui.Base
{
    public abstract class ObservableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetPropertyValue<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
            {
                return;
            }
            property = value;
            NotifyPropertyChanged(propertyName);
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
