// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyCustomSchemeProvider.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using CefSharp;

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
                        return ResourceHandler.FromString("<h3>Invalid HTML String</h3>");
                    }

                    var postData = request.PostData.Elements.FirstOrDefault();
                    var markdown = postData.GetBody();
                    return ResourceHandler.FromString(markdown);
                default:
                    return ResourceHandler.FromString("<h1>Unknown Host</h1>");
            }
        }
    }
}
