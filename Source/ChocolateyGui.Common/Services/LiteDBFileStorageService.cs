// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LiteDBFileStorageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LiteDB;

namespace ChocolateyGui.Common.Services
{
    public class LiteDBFileStorageService : IFileStorageService
    {
        private readonly LiteDatabase _database;

        public LiteDBFileStorageService(LiteDatabase database)
        {
            _database = database;
        }

        public void DeleteAllFiles()
        {
            var files = _database.FileStorage.FindAll();

            foreach (var file in files)
            {
                _database.FileStorage.Delete(file.Id);
            }
        }
    }
}