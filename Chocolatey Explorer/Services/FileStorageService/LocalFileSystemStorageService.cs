using System.IO;
using System.Xml.Linq;

namespace Chocolatey.Explorer.Services.FileStorageService
{
	public class LocalFileSystemStorageService : IFileStorageService
	{
		public string[] GetDirectories(string path)
		{
            this.Log().Debug("Return directories for path: {0}", path);
			return Directory.GetDirectories(path);
		}
		
		public bool DirectoryExists(string path)
		{
            this.Log().Debug("Check if directory exists; {0}", path);
			return Directory.Exists(path);
		}
		
		public XDocument LoadXDocument(string filename)
		{
            this.Log().Debug("Load document: {0}", filename);
		    return File.Exists(filename) ? XDocument.Load(filename) : null;
		}
	}
}
