// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="RemotePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ChocolateyGui.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using ChocolateyGui.Models;
    using ChocolateyGui.Services.PackageServices;
    using ChocolateyGui.ViewModels.Items;

    public class RemotePackageService : IRemotePackageService
    {
        private readonly IProgressService _progressService;
        private readonly ISourceService _sourceService;
        private readonly Func<IPackageViewModel> _packageFactory;
        private readonly ILogService _logService;

        public RemotePackageService(
            IProgressService progressService, 
            ISourceService sourceService, 
            Func<IPackageViewModel> packageFactory, 
            Func<Type, ILogService> logFunc)
        {
            if (logFunc == null)
            {
                throw new ArgumentNullException("logFunc");
            }

            _progressService = progressService;
            _sourceService = sourceService;
            _packageFactory = packageFactory;
            _logService = logFunc(typeof(RemotePackageService));
        }

        public async Task<PackageSearchResults> Search(string query, Uri source = null)
        {
            return await Search(query, new PackageSearchOptions(), source);
        }

        public async Task<PackageSearchResults> Search(string query, PackageSearchOptions options, Uri source = null)
        {
            await _progressService.StartLoading("Search");
            if (string.IsNullOrWhiteSpace(query))
            {
                _progressService.WriteMessage("Loading data...");
            }
            else
            {
                _progressService.WriteMessage(string.Format("Searching for {0}", query));
            }

            if (source == null)
            {
                source = new Uri(_sourceService.GetDefaultSource().Url);
            }

            if (source.Scheme == "http" || source.Scheme == "https")
            {
                var results = await ODataRemotePackageService.Search(query, _packageFactory, options, source);
                await _progressService.StopLoading();
                return results;
            }

            if (source.IsFile || source.IsUnc)
            {
                await _progressService.StopLoading();
                return null;
            }

            throw new InvalidDataException("Invalid Source Uri. Double check that you current source is a valid endpoint.");
        }

        public async Task<IPackageViewModel> GetLatest(string id, bool includePrerelease = false, Uri source = null)
        {
            _progressService.WriteMessage(string.Format("Getting latest version of {0}...", id));
            if (source == null)
            {
                var defaultSource = new Uri(_sourceService.GetDefaultSource().Url);

                if (defaultSource.Scheme == "http" || defaultSource.Scheme == "https")
                {
                    var result = await ODataRemotePackageService.GetLatest(id, _packageFactory, defaultSource, includePrerelease);
                    if (result != null)
                    {
                        result.Source = defaultSource;
                        return result;
                    }
                }

                foreach (var sourceViewModel in _sourceService.GetSources())
                {
                    var currentSource = new Uri(sourceViewModel.Url);
                    if (currentSource.Scheme == "http" || currentSource.Scheme == "https")
                    {
                        var result = await ODataRemotePackageService.GetLatest(id, _packageFactory, currentSource, includePrerelease);
                        if (result == null)
                        {
                            continue;
                        }

                        result.Source = currentSource;
                        await _progressService.StopLoading();
                        return result;
                    }
                }
            }
            else
            {
                if (source.Scheme == "http" || source.Scheme == "https")
                {
                    var result = await ODataRemotePackageService.GetLatest(id, _packageFactory, source, includePrerelease);
                    if (result != null)
                    {
                        result.Source = source;
                    }

                    return result;
                }

                if (source.IsFile || source.IsUnc)
                {
                    return null;
                }
            }

            return null;
        }

        public async Task<IPackageViewModel> EnsureIsLoaded(IPackageViewModel viewModel, Uri source = null)
        {
            await _progressService.StartLoading("Loading Package Information");
            _progressService.WriteMessage("Loading remote package information...");

            // If we don't have a source, iterate through our source until we find one that matches.
            if (source == null)
            {
                var defaultSourceVm = _sourceService.GetDefaultSource();
                var defaultSource = new Uri(defaultSourceVm.Url);
                if (defaultSource.Scheme == "http" || defaultSource.Scheme == "https")
                {
                    var result = await ODataRemotePackageService.EnsureIsLoaded(viewModel, defaultSource);
                    if (result != null)
                    {
                        await _progressService.StopLoading();
                        result.Source = defaultSource;
                        return result;
                    }
                }

                foreach (var sourceViewModel in _sourceService.GetSources())
                {
                    var currentSource = new Uri(sourceViewModel.Url);
                    if (currentSource.Scheme == "http" || currentSource.Scheme == "https")
                    {
                        var result = await ODataRemotePackageService.EnsureIsLoaded(viewModel, currentSource);
                        if (result == null)
                        {
                            continue;
                        }

                        await _progressService.StopLoading();
                        result.Source = currentSource;
                        return result;
                    }
                }
            }
            else
            {
                if (source.Scheme == "http" || source.Scheme == "https")
                {
                    var result = await ODataRemotePackageService.EnsureIsLoaded(viewModel, source);
                    await _progressService.StopLoading();
                    if (result != null)
                    {
                        result.Source = source;
                    }

                    return result;
                }

                if (source.IsFile || source.IsUnc)
                {
                    await _progressService.StopLoading();
                    return null;
                }
            }

            await _progressService.StopLoading();
            return null;
        }

        public async Task<bool> TestSourceUrl(Uri source)
        {
            if (source.Scheme == "http" || source.Scheme == "https")
            {
                var exception = await ODataRemotePackageService.TestPath(source);
                if (exception == null)
                {
                    return true;
                }

                _logService.Debug("TestSourceUrl failed.", exception);
                return false;
            }

            if (source.IsFile || source.IsUnc)
            {
                return false;
            }

            return false;
        }
    }
}