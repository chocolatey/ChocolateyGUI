// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyGuiFeature.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChocolateyGui.Common.Models
{
    public class ChocolateyGuiFeature : INotifyPropertyChanged
    {
        private string _title;
        private bool _enabled;
        private string _description;
        private string _displayTitle;

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayTitle
        {
            get
            {
                return _displayTitle ?? Title;
            }

            set
            {
                _displayTitle = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
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