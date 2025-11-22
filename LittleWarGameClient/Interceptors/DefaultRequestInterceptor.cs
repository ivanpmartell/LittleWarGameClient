using CefSharp;
using CefSharp.Handler;
using LittleWarGameClient.UI;

namespace LittleWarGameClient.Interceptors
{
    internal class DefaultRequestInterceptor : RequestHandler
    {
        protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            GameForm.Instance.requestCallCounter++;
            return base.OnBeforeBrowse(chromiumWebBrowser, browser, frame, request, userGesture, isRedirect);
        }
    }
}
