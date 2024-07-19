using Microsoft.Web.WebView2.WinForms;

namespace LittleWarGameClient
{
    internal class KeyboardHandler
    {
        readonly WebView2 webView;
        readonly Fullscreen fullScreen;
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
                        ElementMessage.CallJSFunc((WebView2)sender, "toggleFriends");
                    e.Handled = true;
                    break;
                case Keys.F10:
                    if (sender != null)
                        ElementMessage.CallJSFunc((WebView2)sender, "toggleMenu");
                    e.Handled = true;
                    break;
                case Keys.F11:
                    if (sender != null)
                        ElementMessage.CallJSFunc((WebView2)sender, "toggleChat");
                    e.Handled = true;
                    break;
            }
        }


        private void TargetWebView_KeyDown(object? sender, KeyEventArgs e)
        {
#if DEBUG
            if (e.KeyData == Keys.F12)
            {
                webView.CoreWebView2.OpenDevToolsWindow();
                e.Handled = true;
                return;
            }
#endif
            switch (e.KeyData)
            {
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                    e.Handled = true;
                    break;
            }
        }
    }
}
