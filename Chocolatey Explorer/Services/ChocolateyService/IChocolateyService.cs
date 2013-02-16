namespace Chocolatey.Explorer.Services.ChocolateyService
{
    public interface IChocolateyService
    {
        event ChocolateyService.OutputDelegate OutputChanged;
        event ChocolateyService.RunFinishedDelegate RunFinished;
        void LatestVersion();
        void Help();
    }
}