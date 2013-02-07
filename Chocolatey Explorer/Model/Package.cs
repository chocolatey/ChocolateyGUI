using System;

namespace Chocolatey.Explorer.Model
{
    public class Package:IComparable
    {
        public String Name { get; set; }
        public String InstalledVersion { get; set; }
        public String Label
        {
            get 
            {
                var installed = InstalledVersion == "no version" ? "x " : "✓";
                return installed + " " + Name;
            }
        }
     
        public new String ToString()
        {
            return Name;
        }
        
        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(Package)) return -1;
            return Name.CompareTo(((Package) obj).Name);
        }

        public bool Equals(Package other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Package)) return false;
            return Equals((Package) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
