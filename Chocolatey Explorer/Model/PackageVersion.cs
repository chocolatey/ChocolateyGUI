using System;

namespace Chocolatey.Explorer.Model
{
    public class PackageVersion
    {
        public String Name { get; set; }
        public String CurrentVersion { get; set; }
        public String Serverversion { get; set; }
        public Boolean IsInstalled { get { return !CurrentVersion.EndsWith("no version"); } }
        public Boolean CanBeUpdated { get { return !CurrentVersion.Equals(Serverversion) && IsInstalled; } }
        public string Description { get; set; }
        public string AuthorName { get; set; }

        public new String ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(Package)) return -1;
            return Name.CompareTo(((Package)obj).Name);
        }

        public bool Equals(PackageVersion other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(PackageVersion)) return false;
            return Equals((PackageVersion)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        } 
    }
}