using System.Xml.Linq;

namespace Chocolatey.Explorer.Services.FileStorageService
{
	public interface IFileStorageService
	{
		string[] GetDirectories(string path);

		bool DirectoryExists(string path);

		XDocument LoadXDocument(string filename);
	}
}
