using CefSharp;
using CefSharp.Handler;

namespace LittleWarGameClient.Interceptors
{
    internal class ContextMenuInterceptor : ContextMenuHandler
    {
        protected override void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();
        }
    }
}
