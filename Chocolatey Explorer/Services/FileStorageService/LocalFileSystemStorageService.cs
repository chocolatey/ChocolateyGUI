using System.IO;
using System.Xml.Linq;

namespace Chocolatey.Explorer.Services.FileStorageService
{
	public class LocalFileSystemStorageService : IFileStorageService
	{
		public string[] GetDirectories(string path)
		{
			return Directory.GetDirectories(path);
		}
		
		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}
		
		public System.Xml.Linq.XDocument LoadXDocument(string filename)
		{
			return XDocument.Load(filename);
		}
	}
}
