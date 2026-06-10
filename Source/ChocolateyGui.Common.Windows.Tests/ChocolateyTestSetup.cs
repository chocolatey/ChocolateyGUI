using AutoMapper;
using ChocolateyGui.Common.Windows.Startup;
using NUnit.Framework;

namespace ChocolateyGui.Common.Windows.Tests
{
    [SetUpFixture]
    public class ChocolateyTestSetup
    {
        public static IMapper Mapper { get; private set; }

        [OneTimeSetUp]
        public void SetupAutomapper()
        {
            var mapperConfiguration = ChocolateyGuiMapper.CreateConfiguration();

            Mapper = mapperConfiguration.CreateMapper();
        }
    }
}