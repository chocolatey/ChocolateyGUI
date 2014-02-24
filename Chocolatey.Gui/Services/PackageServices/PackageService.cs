using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Chocolatey.Gui.ChocolateyFeedService;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.Properties;
using Chocolatey.Gui.Services.PackageServices;
using Chocolatey.Gui.ViewModels.Items;
using Chocolatey.Gui.Utilities.Extensions;

namespace Chocolatey.Gui.Services
{
    public class PackageService : IPackageService
    {
        private readonly IProgressService _progressService;
        private readonly ISourceService _sourceService;
        private readonly Func<IPackageViewModel> _packageFactory;
        private readonly ILogService _logService;

        public PackageService(IProgressService progressService, ISourceService sourceService,
            Func<IPackageViewModel> packageFactory, Func<Type, ILogService> logFunc)
        {
            _progressService = progressService;
            _sourceService = sourceService;
            _packageFactory = packageFactory;
            _logService = logFunc(typeof (PackageService));
        }

        public async Task<PackageSearchResults> Search(string query, Uri source = null)
        {
            return await Search(query, new PackageSearchOptions(), source);
        }

        public async Task<PackageSearchResults> Search(string query, PackageSearchOptions options, Uri source = null)
        {
            _progressService.StartLoading("Search", "Searching for matching packages...");
            if (source == null)
            {
                source = new Uri(_sourceService.GetDefaultSource().Url);
            }

            if (source.Scheme == "http" || source.Scheme == "https")
            {
                var results = await ODataPackageService.Search(query, _packageFactory, options, source);
                _progressService.StopLoading();
                return results;
            }

            if(source.IsFile || source.IsUnc)
            {
                _progressService.StopLoading();
                return null;
            }
            throw new InvalidDataException("Invalid Source Uri. Double check that you current source is a valid endpoint.");
        }

        public IPackageViewModel GetLatest(string id, bool includePrerelease = false, Uri source = null)
        {
            if (source == null)
            {
                var defaultSource = new Uri(_sourceService.GetDefaultSource().Url);

                if (defaultSource.Scheme == "http" || defaultSource.Scheme == "https")
                {
                    var results = ODataPackageService.GetLatest(id, _packageFactory, defaultSource, includePrerelease);
                    return results;
                }
                

                foreach (var sourceViewModel in _sourceService.GetSources())
                {
                    var currentSource = new Uri(sourceViewModel.Url);
                    if (currentSource.Scheme == "http" || currentSource.Scheme == "https")
                    {
                        var result = ODataPackageService.GetLatest(id, _packageFactory, currentSource, includePrerelease);
                        if (result == null)
                            continue;

                        result.Source = currentSource;
                        _progressService.StopLoading();
                        return result;
                    }
                }
            }
            else
            {
                if (source.Scheme == "http" || source.Scheme == "https")
                {
                    var result = ODataPackageService.GetLatest(id, _packageFactory, source, includePrerelease);
                    if(result != null)
                        result.Source = source;
                    return result;
                }

                if (source.IsFile || source.IsUnc)
                {
                    return null;
                }
            }
            return null;
        }

        public async Task<IPackageViewModel> EnsureIsLoaded(IPackageViewModel vm, Uri source = null)
        {
            _progressService.StartLoading("", "Loading package information.");

            // If we don't have a source, iterate through our source until we find one that matches.
            if (source == null)
            {
                var defaultSourceVm = _sourceService.GetDefaultSource();
                var defaultSource = new Uri(defaultSourceVm.Url);
                if (defaultSource.Scheme == "http" || defaultSource.Scheme == "https")
                {
                    var result = await ODataPackageService.EnsureIsLoaded(vm, defaultSource);
                    if (result != null)
                    {
                        _progressService.StopLoading();
                        result.Source = defaultSource;
                        return result;
                    }
                }

                foreach (var sourceViewModel in _sourceService.GetSources())
                {
                    var currentSource = new Uri(sourceViewModel.Url);
                    if (currentSource.Scheme == "http" || currentSource.Scheme == "https")
                    {
                        var result = await ODataPackageService.EnsureIsLoaded(vm, currentSource);
                        if(result == null)
                            continue;

                        _progressService.StopLoading();
                        result.Source = currentSource;
                        return result;
                    }
                }
            }
            else
            {
                if (source.Scheme == "http" || source.Scheme == "https")
                {
                    var result = await ODataPackageService.EnsureIsLoaded(vm, source);
                    _progressService.StopLoading();
                    if (result != null)
                        result.Source = source;
                    return result;
                }

                if (source.IsFile || source.IsUnc)
                {
                    _progressService.StopLoading();
                    return null;
                }
            }
            _progressService.StopLoading();
            return null;
        }

        public async Task<bool> TestSourceUrl(Uri source)
        {

            if (source.Scheme == "http" || source.Scheme == "https")
            {
                var exception =  await ODataPackageService.TestPath(source);
                if (exception == null)
                    return true;

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
