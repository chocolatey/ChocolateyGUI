using System;

namespace Chocolatey.Explorer.Model
{
    public class Package:IComparable
    {
        public String Name { get; set; }
        public String InstalledVersion { get; set; }
        public bool IsInstalled
        {
            get 
            {
                return InstalledVersion != strings.not_available;
            }
        }
     
        public new String ToString()
        {
            return Name;
        }
        
        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(Package)) return -1;
            return System.String.Compare(Name, ((Package) obj).Name, System.StringComparison.Ordinal);
        }

        public bool Equals(Package other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof (Package) && Equals((Package) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
