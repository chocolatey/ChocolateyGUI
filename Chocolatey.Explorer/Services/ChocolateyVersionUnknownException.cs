using System;

namespace Chocolatey.Explorer.Services
{
	public class ChocolateyVersionUnknownException : Exception
	{
		public ChocolateyVersionUnknownException(string versionString) : base("Unknown input received for version: " + versionString) { }
	}
}
