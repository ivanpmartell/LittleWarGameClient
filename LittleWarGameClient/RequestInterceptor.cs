using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleWarGameClient
{
    internal class RequestInterceptor : CefSharp.Handler.RequestHandler
    {
        protected override IResourceRequestHandler? GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            if (request.Url == $"{GameForm.baseUrl}/js/lwg-5.0.0.js")
            {
                return new OverrideJavascript();
            }
            //Default behaviour, url will be loaded normally.
            return null;
        }
    }

    internal class OverrideJavascript : CefSharp.Handler.ResourceRequestHandler
    {
        protected override IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            FileStream fs = File.Open("js/lwg-5.0.0.js", FileMode.Open);
            return ResourceHandler.FromStream(fs, mimeType: Cef.GetMimeType("js"), true);
        }
    }
}
