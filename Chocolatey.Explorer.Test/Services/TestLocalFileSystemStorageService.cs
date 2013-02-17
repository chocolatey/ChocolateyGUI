using System;
using Chocolatey.Explorer.Services.FileStorageService;
using NUnit.Framework;

namespace Chocolatey.Explorer.Test.Services
{
    public class TestLocalFileSystemStorageService
    {
        private IFileStorageService _fileStorage;

        [SetUp]
        public void Setup()
        {
            _fileStorage = new LocalFileSystemStorageService();    
        }

        [Test]
        public void IfDirectoryExistsReturnsTrueForCurrentDirectory()
        {
            Assert.IsTrue(_fileStorage.DirectoryExists(Environment.CurrentDirectory));
        }

        [Test]
        public void IfDirectoryExistsReturnsFalsForNonExstingDirectory()
        {
            Assert.IsFalse(_fileStorage.DirectoryExists(""));
        }

        [Test]
        public void IfGetDirectoriesReturnsDirectoriesForParentOfCurrentDirectory()
        {
            Assert.Greater(_fileStorage.GetDirectories(System.IO.Directory.GetParent(Environment.CurrentDirectory).FullName).Length, 0);
        }

        [Test]
        public void CanLoadXmlDocument()
        {
            Assert.IsNotNull(_fileStorage.LoadXDocument("sources.xml"));
        }

        [Test]
        public void ReturnsNullIfFileDoesNotExists()
        {
            Assert.IsNull(_fileStorage.LoadXDocument(""));
        }
    }
}