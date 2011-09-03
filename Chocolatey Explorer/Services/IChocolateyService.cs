namespace Chocolatey.Explorer.Services
{
    public interface IChocolateyService
    {
        event ChocolateyService.OutputDelegate OutputChanged;
        event ChocolateyService.RunFinishedDelegate RunFinished;
        void LatestVersion();
        void Help();
    }
}