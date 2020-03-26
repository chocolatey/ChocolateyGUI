// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ShowSourcesMessage.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Models.Messages
{
    public class ShowSourcesMessage
    {
        public ShowSourcesMessage(string sourceId = null)
        {
            SourceId = sourceId;
        }

        public string SourceId { get; }
    }
}