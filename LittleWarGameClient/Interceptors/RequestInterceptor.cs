using CefSharp;
using CefSharp.Handler;
using System.Text.RegularExpressions;

namespace LittleWarGameClient.Interceptors
{
    internal class RequestInterceptor : RequestHandler
    {
        protected override IResourceRequestHandler? GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            if (!request.Url.EndsWith(".js"))
                return null;
            string gameplayCodePathRegex = "/play/js/lwg-([\\d+]+\\.)+js";
            Match match = Regex.Match(request.Url, gameplayCodePathRegex);
            if (match.Success)
            {
                return new OverrideJavascript();
            }
            //Default behaviour, url will be loaded normally.
            return null;
        }

        protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            GameForm.Instance.requestCallCounter++;
            return base.OnBeforeBrowse(chromiumWebBrowser, browser, frame, request, userGesture, isRedirect);
        }
    }

    internal class OverrideJavascript : ResourceRequestHandler
    {
        protected override IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            FileStream fs = File.Open("js/lwg.js", FileMode.Open);
            return ResourceHandler.FromStream(fs, mimeType: Cef.GetMimeType("js"), true);
        }
    }
}
