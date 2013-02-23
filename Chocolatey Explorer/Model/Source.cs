using System;

namespace Chocolatey.Explorer.Model
{
    public class Source
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public override string ToString()
        {
            return Name + " (" + Url + ")";
        }

        public bool Equals(Source other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof (Source) && Equals((Source) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}