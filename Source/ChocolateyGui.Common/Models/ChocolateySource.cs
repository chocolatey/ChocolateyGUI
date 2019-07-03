// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateySource.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ChocolateyGui.Common.Models
{
    public class ChocolateySource : IEquatable<ChocolateySource>
    {
        public ChocolateySource(ChocolateySource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Id = source.Id;
            Value = source.Value;
            Disabled = source.Disabled;
            UserName = source.UserName;
            Password = source.Password;
            Priority = source.Priority;
            Certificate = source.Certificate;
            CertificatePassword = source.CertificatePassword;
            BypassProxy = source.BypassProxy;
            AllowSelfService = source.AllowSelfService;
            VisibleToAdminsOnly = source.VisibleToAdminsOnly;
        }

        public ChocolateySource()
        {
        }

        public string Id { get; set; }

        public string Value { get; set; }

        public bool Disabled { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool HasPassword
        {
            get { return !string.IsNullOrWhiteSpace(Password); }
        }

        public int Priority { get; set; }

        public string Certificate { get; set; }

        public string CertificatePassword { get; set; }

        public bool HasCertificatePassword
        {
            get { return !string.IsNullOrWhiteSpace(CertificatePassword); }
        }

        public bool BypassProxy { get; set; }

        public bool AllowSelfService { get; set; }

        public bool VisibleToAdminsOnly { get; set; }

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
                && BypassProxy == other.BypassProxy
                && AllowSelfService == other.AllowSelfService
                && VisibleToAdminsOnly == other.VisibleToAdminsOnly;
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
                hashCode = (hashCode * 397) ^ BypassProxy.GetHashCode();
                hashCode = (hashCode * 397) ^ AllowSelfService.GetHashCode();
                hashCode = (hashCode * 397) ^ VisibleToAdminsOnly.GetHashCode();
                return hashCode;
            }
        }
    }
}