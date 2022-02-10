// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyGuiSetting.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChocolateyGui.Common.Models
{
    public class ChocolateyGuiSetting : INotifyPropertyChanged
    {
        private string _key;
        private string _value;
        private string _description;
        private string _displayTitle;

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayName
        {
            get
            {
                return _displayTitle ?? Key;
            }

            set
            {
                _displayTitle = value;
                OnPropertyChanged();
            }
        }

        public string Key
        {
            get
            {
                return _key;
            }

            set
            {
                _key = value;
                OnPropertyChanged();
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}