using System;
using Chocolatey.Explorer.Model;

namespace Chocolatey.Explorer.Services.SourceService
{
    public interface ISourceService
    {
        event SourceService.SourcesDelegate SourcesChanged;
        event SourceService.CurrentSourceDelegate CurrentSourceChanged;
        void SetCurrentSource(Source source);
        String Source { get; }
        void AddSource(Source source);
    }
}