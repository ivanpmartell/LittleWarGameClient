using Microsoft.Web.WebView2.WinForms;

namespace LittleWarGameClient
{
    internal class KeyboardHandler
    {
        WebView2 webView;
        Fullscreen fullScreen;
        internal KeyboardHandler(WebView2 wv, Fullscreen fs)
        {
            fullScreen = fs;
            webView = wv;
#if DEBUG
            webView.KeyDown += TargetWebView_KeyDown;
#endif
            webView.KeyUp += TargetWebView_KeyUp;
        }

        private void TargetWebView_KeyUp(object? sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F8:
                    fullScreen.Toggle();
                    e.Handled = true;
                    break;
                case Keys.F9:
                    if (sender != null)
                        CallJSFunc(sender, "toggleFriends");
                    e.Handled = true;
                    break;
                case Keys.F10:
                    if (sender != null)
                        CallJSFunc(sender, "toggleMenu");
                    e.Handled = true;
                    break;
            }
        }

#if DEBUG
        private void TargetWebView_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F12:
                    webView.CoreWebView2.OpenDevToolsWindow();
                    break;
            }
    }
#endif

        private void CallJSFunc(object sender, string func, string args = "")
        {
            var script = $"addons.{func}({args})";
            ((WebView2)sender).CoreWebView2.ExecuteScriptAsync(script);
        }
    }
}
