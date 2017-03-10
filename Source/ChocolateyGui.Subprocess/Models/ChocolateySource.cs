// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateySource.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ChocolateyGui.Subprocess.Models
{
    public class ChocolateySource : IEquatable<ChocolateySource>
    {
        public string Id { get; set; }

        public string Value { get; set; }

        public bool Disabled { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int Priority { get; set; }

        public string Certificate { get; set; }

        public string CertificatePassword { get; set; }

        public bool ByPassProxy { get; set; }

        public bool SelfService { get; set; }

        public bool Equals(ChocolateySource other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Id, other.Id)
                && string.Equals(Value, other.Value)
                && Disabled == other.Disabled
                && string.Equals(UserName, other.UserName)
                && string.Equals(Password, other.Password)
                && Priority == other.Priority
                && string.Equals(Certificate, other.Certificate)
                && string.Equals(CertificatePassword, other.CertificatePassword)
                && ByPassProxy == other.ByPassProxy
                && SelfService == other.SelfService;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((ChocolateySource)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Value?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Disabled.GetHashCode();
                hashCode = (hashCode * 397) ^ (UserName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Password?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Priority;
                hashCode = (hashCode * 397) ^ (Certificate?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (CertificatePassword?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ ByPassProxy.GetHashCode();
                hashCode = (hashCode * 397) ^ SelfService.GetHashCode();
                return hashCode;
            }
        }
    }
}