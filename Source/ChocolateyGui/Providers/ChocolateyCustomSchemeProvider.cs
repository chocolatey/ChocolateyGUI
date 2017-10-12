// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyCustomSchemeProvider.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using CefSharp;
using ChocolateyGui.Properties;

namespace ChocolateyGui.Providers
{
    public class ChocolateyCustomSchemeProvider : ISchemeHandlerFactory
    {
        public const string SchemeName = "choco";

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            var uri = new Uri(request.Url);
            switch (uri.Host)
            {
                case "markdown":
                    if (request.PostData == null)
                    {
                        return ResourceHandler.FromString(Resources.ChocolateyCustomSchemeProvider_InvalidHtml);
                    }

                    var postData = request.PostData.Elements.FirstOrDefault();
                    var markdown = postData.GetBody();
                    return ResourceHandler.FromString(markdown);
                default:
                    return ResourceHandler.FromString(Resources.ChocolateyCustomSchemeProvider_UnknownHost);
            }
        }
    }
}