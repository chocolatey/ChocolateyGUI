using System;
using System.IO;
using Chocolatey.Explorer.Services.FileStorageService;
using Chocolatey.Explorer.Services.LogsService;
using NUnit.Framework;
using Rhino.Mocks;

namespace Chocolatey.Explorer.Test.Services
{
    public class TestLogsService
    {
         [Test]
         public void CallsOnRunstartedOnGetLogs()
         {
             var fileStorage = MockRepository.GenerateMock<IFileStorageService>();
             var sut = new LogsService(fileStorage);
             var result = 0;
             sut.RunStarted += start => result = 1;
             sut.GetLogs();
             Assert.AreEqual(1, result);
         }

         [Test]
         public void CallsOnRunFinishedOnGetLogs()
         {
             var fileStorage = MockRepository.GenerateMock<IFileStorageService>();
             var sut = new LogsService(fileStorage);
             var result = 0;
             sut.RunFinished += start => result = 1;
             sut.GetLogs();
             Assert.AreEqual(1, result);
         }

         [Test]
         public void CallsCorrectDirectory()
         {
             var fileStorage = MockRepository.GenerateMock<IFileStorageService>();
             var sut = new LogsService(fileStorage);
             sut.GetLogs();
             fileStorage.AssertWasCalled(x => x.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ChocolateyGUI", "Logs")));
         }
    }
}