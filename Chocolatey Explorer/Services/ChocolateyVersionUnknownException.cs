using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chocolatey.Explorer.Services
{
	public class ChocolateyVersionUnknownException : Exception
	{
		public ChocolateyVersionUnknownException(string versionString) : base("Unknown input received for version: " + versionString) { }
	}
}
