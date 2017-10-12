// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ShowSourcesMessage.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models.Messages
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