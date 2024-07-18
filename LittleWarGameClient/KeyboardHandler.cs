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
            webView.KeyDown += TargetWebView_KeyDown;
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
                case Keys.F11:
                    if (sender != null)
                        CallJSFunc(sender, "toggleChat");
                    e.Handled = true;
                    break;
            }
        }


        private void TargetWebView_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
#if DEBUG
                case Keys.F12:
                    webView.CoreWebView2.OpenDevToolsWindow();
                    break;
#endif
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                    e.Handled = true;
                    break;
            }
        }

        private void CallJSFunc(object sender, string func, string args = "")
        {
            var script = $"addons.{func}({args})";
            ((WebView2)sender).CoreWebView2.ExecuteScriptAsync(script);
        }
    }
}
