// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyAggregatedSources.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Properties;

namespace ChocolateyGui.Models
{
    public class ChocolateyAggregatedSources : ChocolateySource
    {
        public ChocolateyAggregatedSources()
        {
            Id = Resources.SourcesView_AggregatedSourcesId;
        }
    }
}