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
        private readonly Func<IPackageViewModel> _packageFactory;

        public PackageService(IProgressService progressService, Func<IPackageViewModel> packageFactory)
        {
            _progressService = progressService;
            _packageFactory = packageFactory;
        }

        public async Task<PackageSearchResults> Search(string query, Uri source = null)
        {
            return await Search(query, new PackageSearchOptions(), source);
        }

        public async Task<PackageSearchResults> Search(string query, PackageSearchOptions options, Uri source = null)
        {
            _progressService.StartLoading();
            if (source == null)
            {
                var currentSource = Settings.Default.currentSource;
                var sources = Settings.Default.sources;
                foreach (var parts in (from string sourceString in sources select sourceString.Split('|')).Where(parts => parts[0] == currentSource))
                {
                    source = new Uri(parts[1]);
                }
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
                var currentSource = Settings.Default.currentSource;
                var sources = Settings.Default.sources;
                foreach (var parts in (from string sourceString in sources select sourceString.Split('|')).Where(parts => parts[0] == currentSource))
                {
                    source = new Uri(parts[1]);
                }
            }

            if (source.Scheme == "http" || source.Scheme == "https")
            {
                var results = ODataPackageService.GetLatest(id, _packageFactory, source, includePrerelease);
                return results;
            }

            if (source.IsFile || source.IsUnc)
            {
                return null;
            }
            return null;
        }

        public async Task<IPackageViewModel> EnsureIsLoaded(IPackageViewModel vm, Uri source = null)
        {
            _progressService.StartLoading();
            if (source == null)
            {
                var currentSource = Settings.Default.currentSource;
                var sources = Settings.Default.sources;
                foreach (var parts in (from string sourceString in sources select sourceString.Split('|')).Where(parts => parts[0] == currentSource))
                {
                    source = new Uri(parts[1]);
                }
            }


            if (source.Scheme == "http" || source.Scheme == "https")
            {
                var results = await ODataPackageService.EnsureIsLoaded(vm, source);
                _progressService.StopLoading();
                return results;
            }

            if (source.IsFile || source.IsUnc)
            {
                _progressService.StopLoading();
                return null;
            }
            _progressService.StopLoading();
            return null;
        }
    }
}
