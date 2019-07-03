// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyAggregatedSources.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Common.Properties;

namespace ChocolateyGui.Common.Models
{
    public class ChocolateyAggregatedSources : ChocolateySource
    {
        public ChocolateyAggregatedSources()
        {
            Id = Resources.SourcesView_AggregatedSourcesId;
        }
    }
}