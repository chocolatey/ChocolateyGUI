using ChocolateyGui.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChocolateyGui.Models
{
    public class ChocolateyAggregatedSources : ChocolateySource
    {
        public ChocolateyAggregatedSources() {
            Id = Resources.SourcesView_AggregatedSourcesId;
        }
    }
}
